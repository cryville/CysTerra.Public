using Cryville.EEW.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.BMKGOpenData {
	public class BMKGOpenDataWorker : HttpPullWorker, ISourceWorker<BMKGEarthquake> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public event Handler<BMKGEarthquake?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		static readonly Dictionary<DataSubtype, Uri> _dataSubtypeMap = new() {
			{ DataSubtype.Major, new("gempaterkini.json", UriKind.Relative) },
			{ DataSubtype.Felt, new("gempadirasakan.json", UriKind.Relative) },
		};
		[LocalizableDisplayName("PNDataSubtypes")]
		[LocalizableDescription("PDDataSubtypes")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		public ISet<DataSubtype> DataSubtypes {
			get => m_dataSubtypes;
			set {
				m_dataSubtypes = value;
				MapDataSubtype();
			}
		}
		void MapDataSubtype() {
			m_dataUris = [.. m_dataSubtypes.Select(i => _dataSubtypeMap.TryGetValue(i, out var uri) ? uri : null).OfType<Uri>()];
		}
		ISet<DataSubtype> m_dataSubtypes = (HashSet<DataSubtype>)[DataSubtype.Major, DataSubtype.Felt];
		Uri[] m_dataUris = [];
		[Obsolete("Use the DataSubtypes property.")]
		public void SetDataUris(IEnumerable<Uri> uris) => m_dataUris = [.. uris];

		public BMKGOpenDataWorker(Uri uri) : base(uri) {
			MapDataSubtype();
		}

		sealed class ListState {
			public bool _init;
			public DateTimeOffset? _lastModified;
			public HashSet<BMKGEarthquake> _history = [];
		}
		readonly Dictionary<Uri, ListState> _history = [];
		BMKGEarthquake? _latest;
#if !DEBUG
		int _phase, _period = 1;
#endif
		protected override async Task Handle(Stream stream, HttpResponseHeaders headers, CancellationToken cancellationToken) {
			var data = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.BMKGEarthquakeLatest, cancellationToken).ConfigureAwait(true) ?? throw new InvalidOperationException("Empty data.");
			var latest = data.EarthquakeInfo.Earthquake;
			if (latest == _latest) return;
			_latest = latest;
#if !DEBUG
			_phase = 0;
			_period = 1;
#endif
		}
		protected override async Task AfterHandled(CancellationToken cancellationToken) {
#if !DEBUG
			if (++_phase < _period) return;
			_phase = 0;
			_period *= 2;
			if (_period > 1440) _period = 1440;
#endif
			foreach (var uri in m_dataUris) {
				if (!_history.TryGetValue(uri, out var state)) {
					_history.Add(uri, state = new());
				}
				await PullList(uri, state, cancellationToken).ConfigureAwait(true);
			}
		}
		async Task PullList(Uri uri, ListState state, CancellationToken cancellationToken) {
			uri = new(BaseUri, uri);
			using var response = await TrySendAsync(
				() => new HttpRequestMessage(HttpMethod.Get, uri) { Headers = { IfModifiedSince = state._lastModified } },
				cancellationToken
			).ConfigureAwait(true);
			if (response == null)
				return;
			if (response.IsSuccessStatusCode)
				state._lastModified = response.Content.Headers.LastModified;
			if (response.StatusCode != HttpStatusCode.OK)
				return;
			using var stream = await response.Content.ReadAsStreamAsync(
#if NET5_0_OR_GREATER
				cancellationToken
#endif
			).ConfigureAwait(true);
			var data = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.BMKGEarthquakeList, cancellationToken).ConfigureAwait(true) ?? throw new InvalidOperationException("Empty data.");
			var result = data.EarthquakeInfo.Earthquakes;
			if (state._init) {
				foreach (var entry in result.Reverse()) {
					if (!state._history.Add(entry)) continue;
					Received?.Invoke(this, entry);
				}
				state._history.RemoveWhere(e => !result.Any(r => r == e));
			}
			else {
				state._init = true;
				foreach (var entry in result) {
					state._history.Add(entry);
				}
			}
		}

		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
	}
}
