using Cryville.EEW.FANStudio.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.FANStudio {
	public class FANStudioWorker<T>(Uri uri) : WebSocketWorker(uri), ISourceWorker<T> where T : class {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(typeof(T).Name, ref culture);

		public event Handler<T>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		readonly HashSet<string> _historySet = [];
		readonly Queue<string> _historyList = [];
		protected override async Task Handle(Stream stream, CancellationToken cancellationToken) {
			try {
				var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.FANStudioMessage, cancellationToken).ConfigureAwait(true) ?? throw new JsonException("Null event.");
				if (e is FANStudioDataMessage msg) {
					var typeInfo = SerializerContext.Default.GetTypeInfo(typeof(T)) ?? throw new NotSupportedException("Message type not supported.");
					var ev = (T)(msg.Data.Deserialize(typeInfo) ?? throw new JsonException("Null event."));
					string hash = msg.MD5 ?? ev.GetHashCode().ToString("x8", CultureInfo.InvariantCulture);
					if (!_historySet.Add(hash)) return;
					_historyList.Enqueue(hash);
					if (e is FANStudioUpdateMessage || _historyList.Count > 1) {
						if (_historyList.Count > 10) {
							_historySet.Remove(_historyList.Dequeue());
						}
						Received?.Invoke(this, ev);
					}
				}
			}
			catch (JsonException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}
		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
	}
}
