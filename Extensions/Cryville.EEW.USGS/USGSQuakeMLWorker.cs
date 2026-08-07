using QuakeML;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Cryville.EEW.USGS {
	public class USGSQuakeMLWorker(Uri uri) : USGSWorker(uri, ".quakeml") {
		public override event Handler<object?>? Received;

		protected override void InvalidateHistory() => _init = false;

		bool _init;
		readonly HashSet<(string, DateTime?)> _history = [];
#if NET5_0_OR_GREATER
		[UnconditionalSuppressMessage("Trimming", "IL2026")]
#endif
		protected override Task Handle(Stream stream, HttpResponseHeaders headers, CancellationToken cancellationToken) {
			using var reader = XmlReader.Create(stream, SharedSettings.XmlReaderSettings);
			Quakeml? quakeml;
			try {
				quakeml = (Quakeml?)Shared.QuakeMLSerializer.Deserialize(reader);
			}
			catch (InvalidOperationException ex) {
				OnError(new SourceWorkerNetworkException("Server returned invalid data.", ex));
				return Task.CompletedTask;
			}
			if (quakeml == null)
				throw new InvalidOperationException("Empty QuakeML.");
			var events = quakeml.eventParameters.@event ?? [];
			if (_init) {
				foreach (var entry in events.Reverse()) {
					if (!_history.Add((entry.publicID, entry.creationInfo?.creationTime))) continue;
					Received?.Invoke(this, entry);
				}
				_history.RemoveWhere(e => !events.Any(r => (r.publicID, r.creationInfo?.creationTime) == e));
			}
			else {
				_init = true;
#if DEBUG
				foreach (var entry in events.Reverse()) {
					Received?.Invoke(this, entry);
				}
#endif
				foreach (var entry in events) {
					_history.Add((entry.publicID, entry.creationInfo?.creationTime));
				}
			}
			return Task.CompletedTask;
		}
	}
}
