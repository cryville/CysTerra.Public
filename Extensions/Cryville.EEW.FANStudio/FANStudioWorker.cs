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
	public class FANStudioWorker<T>(Uri uri, JsonTypeInfo typeInfo) : WebSocketWorker(uri), ISourceWorker<T> where T : class {
		public virtual string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(typeof(T).Name, ref culture);

		public event Handler<T>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

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
		}

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
	}
}
