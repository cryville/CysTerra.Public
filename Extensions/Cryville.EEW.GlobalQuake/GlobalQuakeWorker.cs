using Cryville.Common.Compat;
using Cryville.Interop.Java.ObjectStream;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.GlobalQuake {
	public class GlobalQuakeWorker(string host, int port) : ISourceWorker<GlobalQuakeReport> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public event Handler<GlobalQuakeReport?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (!disposing) return;
		}

		public async Task RunAsync(CancellationToken cancellationToken) {
			try {
				int reconnectTimeout = 1000;
				for (; ; ) {
					using var client = new TcpClient();
					using var ctsTimeout = new CancellationTokenSource(10000);
					var ctTimeout = ctsTimeout.Token;
					try {
#if NET5_0_OR_GREATER
						using var ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(ctTimeout, cancellationToken);
						await client.ConnectAsync(host, port, ctsLinked.Token).ConfigureAwait(true);
#else
						await client.ConnectAsync(host, port).ConfigureAwait(true);
#endif
						RunAsyncCore(client, cancellationToken);
					}
					catch (SocketException ex) {
						ErrorEmitted?.Invoke(this, new SourceWorkerNetworkException(ex.Message, ex));
					}
					catch (IOException ex) {
						ErrorEmitted?.Invoke(this, new SourceWorkerNetworkException(ex.Message, ex));
					}
					catch (OperationCanceledException ex) {
						if (cancellationToken.IsCancellationRequested) {
							client.Close();
							break;
						}
						else if (ctTimeout.IsCancellationRequested) {
							ErrorEmitted?.Invoke(this, new SourceWorkerNetworkException("Connection timed out.", ex));
						}
						else throw;
					}
					await Task.Delay(reconnectTimeout, cancellationToken).ConfigureAwait(true);
					reconnectTimeout *= 2;
					if (reconnectTimeout > 30000) reconnectTimeout = 30000;
				}
			}
			catch (OperationCanceledException) {
				if (!cancellationToken.IsCancellationRequested) {
					throw;
				}
			}
			catch (Exception) {
				throw;
			}
		}

		protected virtual string APINamespace => "gqserver.api";
		protected virtual void RunAsyncCore(TcpClient client, CancellationToken cancellationToken) {
			ThrowHelper.ThrowIfNull(client);

			Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
			using var stream = client.GetStream();
			using var writer = new ObjectStreamWriter(stream);
			Handshake(writer);

			var oEarthquakesRequestPacket = new SerializedJavaObject {
				ClassDesc = new SerializedJavaClassDesc(APINamespace + ".packets.earthquake.EarthquakesRequestPacket") {
					ClassDescFlags = JavaClassDescFlags.Serializable,
				}
			};
			writer.Write(oEarthquakesRequestPacket);

			using var cts = new CancellationTokenSource();
			using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
			var heartbeatTask = Task.Run(() => SendHeartbeat(writer, TimeSpan.FromSeconds(20), linkedCts.Token), linkedCts.Token);
			try {
				using var reader = CreateAndVerifyStreamReader(stream);
				var context = new ObjectStreamContext();
				for (; ; ) {
					if (cancellationToken.IsCancellationRequested) break;
					var o = reader.ReadContent();
					if (cancellationToken.IsCancellationRequested) break;
					if (o is SerializedObjectStreamReset) {
						context.Reset();
						continue;
					}
					if (o is not SerializedJavaObject obj) continue;
					if (obj.ClassDesc is not SerializedJavaClassDesc classDesc) continue;
					if (classDesc.Name == APINamespace + ".packets.system.TerminationPacket")
						throw new InvalidOperationException($"Connection terminated by server: {context.GetValue(obj, "cause")}");
					HandleContentPacket(context, obj, classDesc);
				}
			}
			finally {
				cts.Cancel();
				heartbeatTask.ContinueWith(task => task.Exception, TaskScheduler.Current).Wait(CancellationToken.None);
			}
		}

		static ObjectStreamReader CreateAndVerifyStreamReader(Stream stream) {
			try {
				return new ObjectStreamReader(stream);
			}
			catch (FormatException ex) {
				using var lres = new LocalizedResource("", SharedCultures.CurrentUICulture);
				var res = lres.RootMessageStringSet;
				throw new SourceWorkerClientException(res.GetStringRequired("ErrorInvalidServer"), ex);
			}
		}

		async Task SendHeartbeat(ObjectStreamWriter writer, TimeSpan period, CancellationToken token) {
			var oHeartbeatPacket = new SerializedJavaObject {
				ClassDesc = new SerializedJavaClassDesc(APINamespace + ".packets.system.HeartbeatPacket") {
					ClassDescFlags = JavaClassDescFlags.Serializable,
				}
			};
			try {
				for (; ; ) {
					writer.Write(oHeartbeatPacket);
					await Task.Delay(period, token).ConfigureAwait(true);
				}
			}
			catch (SocketException) { }
			catch (IOException) { }
			catch (OperationCanceledException) { }
		}

		protected virtual int ProtocolVersion => 9;
		void Handshake(ObjectStreamWriter writer) {
			var cHandshakePacket = new SerializedJavaClassDesc(APINamespace + ".packets.system.HandshakePacket") {
				ClassDescFlags = JavaClassDescFlags.Serializable,
			};
			cHandshakePacket.Fields.Add(new SerializedJavaPrimitiveField("compatVersion", JavaPrimitiveType.Integer));
			cHandshakePacket.Fields.Add(new SerializedJavaObjectField("clientConfig", false, "L" + APINamespace.Replace('.', '/') + "/data/system/ServerClientConfig;"));
			var oHandshakePacket = new SerializedJavaObject {
				ClassDesc = cHandshakePacket,
			};
			oHandshakePacket.Values.Add(ProtocolVersion);
			var cServerClientConfig = new SerializedJavaClassDesc(APINamespace + ".data.system.ServerClientConfig") {
				ClassDescFlags = JavaClassDescFlags.Serializable,
			};
			cServerClientConfig.Fields.Add(new SerializedJavaPrimitiveField("earthquakeData", JavaPrimitiveType.Boolean));
			cServerClientConfig.Fields.Add(new SerializedJavaPrimitiveField("stationData", JavaPrimitiveType.Boolean));
			var oServerClientConfig = new SerializedJavaObject {
				ClassDesc = cServerClientConfig,
			};
			oServerClientConfig.Values.Add(true);
			oServerClientConfig.Values.Add(false);
			oHandshakePacket.Values.Add(oServerClientConfig);
			writer.Write(oHandshakePacket);
		}

		protected virtual void HandleContentPacket(ObjectStreamContext context, SerializedJavaObject obj, SerializedJavaClassDesc classDesc) {
			ThrowHelper.ThrowIfNull(classDesc);
			var type = classDesc.Name;
			if (!type.StartsWith(APINamespace, StringComparison.Ordinal)) {
				throw new InvalidOperationException("");
			}
			switch (type.AsSpan()[APINamespace.Length..]) {
				case ".packets.earthquake.HypocenterDataPacket":
					HandleHypocenterDataPacket(context, obj);
					break;
				case ".packets.earthquake.ArchivedQuakePacket":
					HandleArchivedQuakePacket(context, obj);
					break;
				case ".packets.earthquake.EarthquakeCheckPacket":
					HandleEarthquakeCheckPacket(context, obj);
					break;
			}
		}

		protected virtual void HandleHypocenterDataPacket(ObjectStreamContext context, SerializedJavaObject obj) {
			ThrowHelper.ThrowIfNull(context);
			var data = (SerializedJavaObject)(context.GetValue(obj, "data") ?? throw new InvalidOperationException("Null hypocenter data."));
			Received?.Invoke(this, new(
				(float)context.GetValue(data, "lat")!,
				(float)context.GetValue(data, "lon")!,
				(float)context.GetValue(data, "depth")!,
				(float)context.GetValue(data, "magnitude")!,
				DateTime.UnixEpoch + TimeSpan.FromMilliseconds((long)context.GetValue(data, "lastUpdate")!),
				DateTime.UnixEpoch + TimeSpan.FromMilliseconds((long)context.GetValue(data, "origin")!),
				(int)context.GetValue(data, "revisionID")!,
				(string)context.GetValue(data, "region")!,
				ToGuid((SerializedJavaObject)context.GetValue(data, "uuid")!, context),
				ExtractHypocenterQualityData(context, obj)
			));
		}

		protected virtual IHypocenterQualityData ExtractHypocenterQualityData(ObjectStreamContext context, SerializedJavaObject obj) {
			ThrowHelper.ThrowIfNull(context);
			var advancedHypocenterData = (SerializedJavaObject)(context.GetValue(obj, "advancedHypocenterData") ?? throw new InvalidOperationException("Null advanced hypocenter data."));
			var qualityData = (SerializedJavaObject)(context.GetValue(advancedHypocenterData, "qualityData") ?? throw new InvalidOperationException("Null hypocenter quality data."));
			return new HypocenterQualityData(
				(float)context.GetValue(qualityData, "errDepth")!,
				(float)context.GetValue(qualityData, "errEW")!,
				(float)context.GetValue(qualityData, "errNS")!,
				(float)context.GetValue(qualityData, "errOrigin")!,
				(float)context.GetValue(qualityData, "pct")!,
				(int)context.GetValue(qualityData, "stations")!
			);
		}

		protected virtual void HandleArchivedQuakePacket(ObjectStreamContext context, SerializedJavaObject obj) {
			ThrowHelper.ThrowIfNull(context);
			var archivedQuakeData = (SerializedJavaObject)(context.GetValue(obj, "archivedQuakeData") ?? throw new InvalidOperationException("Null archived quake data."));
			Received?.Invoke(this, new(
				(float)context.GetValue(archivedQuakeData, "lat")!,
				(float)context.GetValue(archivedQuakeData, "lon")!,
				(float)context.GetValue(archivedQuakeData, "depth")!,
				(float)context.GetValue(archivedQuakeData, "magnitude")!,
				DateTime.UnixEpoch + TimeSpan.FromMilliseconds((long)context.GetValue(archivedQuakeData, "finalUpdateMillis")!),
				DateTime.UnixEpoch + TimeSpan.FromMilliseconds((long)context.GetValue(archivedQuakeData, "origin")!),
				0, null,
				ToGuid((SerializedJavaObject)context.GetValue(archivedQuakeData, "uuid")!, context),
				new HypocenterQualityClass((byte)context.GetValue(archivedQuakeData, "qualityID")!),
				true
			));
		}

		protected virtual void HandleEarthquakeCheckPacket(ObjectStreamContext context, SerializedJavaObject obj) {
			ThrowHelper.ThrowIfNull(context);
			var info = (SerializedJavaObject)(context.GetValue(obj, "info") ?? throw new InvalidOperationException("Null earthquake info."));
			var revId = (int)context.GetValue(info, "revisionID")!;
			if (revId == -1) {
				Received?.Invoke(this, new(
					0, 0, 0, 0, default, default, -1, null,
					ToGuid((SerializedJavaObject)context.GetValue(info, "uuid")!, context), null
				));
			}
		}

		static unsafe Guid ToGuid(SerializedJavaObject uuid, ObjectStreamContext context) {
			long* ptr = stackalloc long[2];
			ptr[0] = (long)context.GetValue(uuid, "leastSigBits")!;
			ptr[1] = (long)context.GetValue(uuid, "mostSigBits")!;
			return *(Guid*)ptr;
		}
	}
}
