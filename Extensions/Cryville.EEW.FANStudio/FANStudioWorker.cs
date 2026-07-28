using Cryville.EEW.FANStudio.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.FANStudio {
	public class FANStudioWorker<T>(Uri uri, string? authKey, JsonTypeInfo typeInfo) : WebSocketWorker(uri), ISourceWorker<T> where T : class {
		public virtual string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(typeof(T).Name, ref culture);

		public event Handler<T>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override async Task OnConnected(CancellationToken cancellationToken) {
			await base.OnConnected(cancellationToken).ConfigureAwait(true);
			if (string.IsNullOrEmpty(authKey))
				return;
			using var stream = new MemoryStream();
			await JsonSerializer.SerializeAsync(stream, new FANStudioAuthMessage(authKey), SerializerContext.Default.FANStudioAuthMessage, cancellationToken).ConfigureAwait(true);
			stream.Position = 0;
			await SendAsync(stream, WebSocketMessageType.Text, cancellationToken).ConfigureAwait(true);
		}

		readonly HashSet<string> _historySet = [];
		readonly Queue<string> _historyList = [];
		protected override async Task Handle(Stream stream, WebSocketMessageType messageType, CancellationToken cancellationToken) {
			try {
				var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.FANStudioMessage, cancellationToken).ConfigureAwait(true) ?? throw new JsonException("Null event.");
				HandleMessage(e);
			}
			catch (JsonException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}

		protected virtual void HandleMessage(FANStudioMessage e) {
			if (e is FANStudioDataMessage msg) {
				var ev = (T)(msg.Data.Deserialize(typeInfo) ?? throw new JsonException("Null event."));
				string hash = msg.MD5 ?? ev.GetHashCode().ToString("x8", CultureInfo.InvariantCulture);
				if (!_historySet.Add(hash))
					return;
				_historyList.Enqueue(hash);
				if (e is FANStudioUpdateMessage || _historyList.Count > 1) {
					if (_historyList.Count > 10) {
						_historySet.Remove(_historyList.Dequeue());
					}
					Received?.Invoke(this, ev);
				}
			}
			else if (e is FANStudioErrorMessage errorMsg) {
				using var lres = new LocalizedResource("", SharedCultures.CurrentUICulture);
				var res = lres.RootMessageStringSet;
				ErrorEmitted?.Invoke(this, new InvalidOperationException(string.Format(SharedCultures.CurrentCulture, res.GetStringRequired("ErrorServer"), errorMsg.Message)));
			}
			else if (e is FANStudioAuthFailureMessage authFailureMessage) {
				using var lres = new LocalizedResource("", SharedCultures.CurrentUICulture);
				var res = lres.RootMessageStringSet;
				throw new SourceWorkerClientException(string.Format(SharedCultures.CurrentCulture, res.GetStringRequired("ErrorAuthFailure"), authFailureMessage.Message));
			}
		}

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
	}
}
