using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.NowQuakeCENCInt {
	public class NowQuakeCENCIntWorkerV2(Uri wsUri) : WebSocketWorker(wsUri), ISourceWorker {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceNameV2(ref culture);

		public event Handler<object?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override async Task Handle(Stream stream, WebSocketMessageType messageType, CancellationToken cancellationToken) {
			try {
				var m = await JsonSerializer.DeserializeAsync(stream, V2SerializerContext.Default.V2Message, cancellationToken).ConfigureAwait(true) ?? throw new JsonException("Null event.");
				var e = V2MessageParser.Parse(m);
				Received?.Invoke(this, e);
			}
			catch (JsonException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
			catch (NotSupportedException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}
		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
	}
}
