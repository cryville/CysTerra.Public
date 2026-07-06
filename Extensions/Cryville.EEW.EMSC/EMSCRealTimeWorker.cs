using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.EMSC {
	public class EMSCRealTimeWorker(Uri uri) : WebSocketWorker(uri), ISourceWorker<EMSCRealTimeAction> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public event Handler<EMSCRealTimeAction?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		readonly HashSet<EMSCRealTimeEvent> _historySet = new(SeismicParameterEqualityComparer.Instance);
		readonly Queue<EMSCRealTimeEvent> _historyList = [];
		protected override async Task Handle(Stream stream, WebSocketMessageType messageType, CancellationToken cancellationToken) {
			try {
				var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.EMSCRealTimeAction, cancellationToken).ConfigureAwait(true)
					?? throw new InvalidOperationException("Null event.");
				if (e.Event is EMSCRealTimeEvent ev) {
					if (!_historySet.Add(ev)) return;
					_historyList.Enqueue(ev);
					if (_historyList.Count > 10) {
						_historySet.Remove(_historyList.Dequeue());
					}
				}
				Received?.Invoke(this, e);
			}
			catch (JsonException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}

		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
	}
}
