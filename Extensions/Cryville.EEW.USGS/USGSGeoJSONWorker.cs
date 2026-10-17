using Cryville.EEW.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.USGS {
	public class USGSGeoJSONWorker : USGSWorker, IUSGSGeoJSONFeedWorker {
		readonly USGSGeoJSONFeedHandler _handler;

		public USGSGeoJSONWorker(Uri uri) : base(uri, ".geojson") {
			_handler = new(this);
			Filter = (HashSet<USGSProductType>)[];
		}

		public override event Handler<object?>? Received;

		[LocalizableDisplayName("PNFilter")]
		[LocalizableDescription("PDFilter")]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		public ISet<USGSProductType> Filter {
			get => _handler.Filter;
			set => _handler.Filter = value;
		}
		[Obsolete("Use the Filter property.")]
		public void SetFilter(IReadOnlyDictionary<string, IEnumerable<string>> filter) {
			_handler._pathFilter = filter.ToDictionary(i => i.Key, i => i.Value.ToHashSet());
		}

		protected override void InvalidateHistory() => _init = false;

		bool _init;
		readonly Dictionary<string, long> _history = [];
		readonly HashSet<string> _setBuffer = [];
		readonly HashSet<string> _historySetBuffer = [];
		protected override async Task Handle(Stream stream, HttpResponseHeaders headers, CancellationToken cancellationToken) {
			var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.USGSEarthquakes, cancellationToken).ConfigureAwait(true)
				?? throw new InvalidOperationException("Null event.");
			_setBuffer.Clear();
			foreach (var feat in e.Features.Reverse()) {
				var props = feat.Properties;
				string id = feat.Id.GetString() ?? throw new FormatException("Missing event ID.");
				_setBuffer.Add(id);
				long updatedTime = props.UpdatedTimestamp;
				if (_history.TryGetValue(id, out var lastUpdatedTime)) {
					if (lastUpdatedTime == updatedTime) continue;
					_history[id] = updatedTime;
				}
				else {
					_history[id] = updatedTime;
#if !DEBUG
					if (!_init) continue;
#endif
				}
				await _handler.Handle(props, cancellationToken).ConfigureAwait(true);
			}
			_historySetBuffer.Clear();
			foreach (var quake in _history) {
				string id = quake.Key;
				if (!_setBuffer.Contains(id)) {
					_historySetBuffer.Add(id);
				}
			}
			foreach (var id in _historySetBuffer) {
				_history.Remove(id);
				_handler.RemoveHistory(id);
			}
			_init = true;
		}

		public Task<HttpResponseMessage?> TryGetAsync(Uri uri, CancellationToken cancellationToken) => TryGetAsync(uri, cancellationToken, HttpRetryStrategies.Report | HttpRetryStrategies.RetryBeforeExit);
		public void OnReceived(object e) => Received?.Invoke(this, e);
		void IUSGSGeoJSONFeedWorker.OnError(Exception ex) => OnError(ex);
	}
}
