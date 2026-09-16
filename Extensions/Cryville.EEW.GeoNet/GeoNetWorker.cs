using Cryville.EEW.ComponentModel;
using Cryville.EEW.GeoJSON;
using Cryville.EEW.GeoNet.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Cryville.EEW.GeoNet {
	public class GeoNetWorker : HttpPullWorker, ISourceWorker, IPropertiesHolder {
		public string? GetName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("SourceName");
		}

		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;
		public event Handler<object?>? Received;

		[LocalizableDisplayName("PNDoGetFullHistory")]
		public bool DoGetFullHistory { get; set; }

		[LocalizableDisplayName("PNDoGetStrongMotionInfo")]
		public bool DoGetStrongMotionInfo { get; set; }

		Uri _uri;
		int m_minimumMMI = 3;
		[LocalizableDisplayName("PNMinimumMMI")]
		public int MinimumMMI {
			get => m_minimumMMI;
			set {
				if (value is < -1 or > 8)
					throw new ArgumentOutOfRangeException(nameof(value));
				m_minimumMMI = value;
				UpdateUri();
				_init = false;
			}
		}
		[MemberNotNull(nameof(_uri))]
		void UpdateUri() {
			_uri = new UriBuilder(BaseUri) {
				Query = "MMI=" + HttpUtility.UrlEncode(m_minimumMMI.ToString(CultureInfo.InvariantCulture))
			}.Uri;
		}
		protected override Uri GetUri() => _uri;

		readonly Uri _historyBaseUri;
		readonly Uri _strongBaseUri;

		public GeoNetWorker(Uri uri, Uri historyBaseUri, Uri strongBaseUri) : base(uri) {
			_historyBaseUri = historyBaseUri;
			_strongBaseUri = strongBaseUri;
			UpdateUri();
		}

		bool _init;
		DateTimeOffset _lastTime;
		readonly Dictionary<string, IFeature<GeoNetQuake>> _quakeHistory = [];
		readonly HashSet<string> _quakeSetBuffer = [];
		readonly HashSet<string> _quakeHistorySetBuffer = [];

		protected override async Task Handle(Stream stream, HttpResponseHeaders headers, CancellationToken cancellationToken) {
			var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.FeatureCollectionGeoNetQuake, cancellationToken).ConfigureAwait(true)
				?? throw new InvalidOperationException("Null event.");
			_quakeSetBuffer.Clear();
			bool flag = false;
			foreach (var quake in e.Features.Reverse().SkipWhile(i => i.Properties.Time < _lastTime)) {
				var props = quake.Properties;
				if (!flag) {
					flag = true;
					_lastTime = props.Time;
				}
				string id = props.PublicID;
				_quakeSetBuffer.Add(id);
				if (_quakeHistory.TryGetValue(id, out var lastQuake)) {
					if (GeoNetQuakeParametersComparer.Instance.Equals(lastQuake, quake)) continue;
					_quakeHistory[id] = quake;
				}
				else {
					_quakeHistory[id] = quake;
					if (!_init) continue;
				}

				if (DoGetFullHistory) {
					await HandleFullHistory(id, cancellationToken).ConfigureAwait(true);
				}
				else {
					Received?.Invoke(this, quake);
				}
				if (DoGetStrongMotionInfo) {
					if (_strongInfoDelay.TryGetValue(id, out var delay)) {
						delay.Reset();
					}
					else if (props.Magnitude >= 4) {
						_strongInfoDelay.Add(id, new());
					}
				}
			}
			_quakeHistorySetBuffer.Clear();
			foreach (var quake in _quakeHistory) {
				string id = quake.Key;
				if (!_quakeSetBuffer.Contains(id)) {
					_quakeHistorySetBuffer.Add(id);
				}
			}
			foreach (var id in _quakeHistorySetBuffer) {
				_quakeHistory.Remove(id);
				_lastModified.Remove(id);
				_strongInfoDelay.Remove(id);
				_strongInfoVersions.Remove(id);
			}
			_init = true;
		}

		readonly Dictionary<string, DateTimeOffset> _lastModified = [];
		async Task HandleFullHistory(string id, CancellationToken cancellationToken) {
			using var response = await TryGetAsync(new(_historyBaseUri, id), cancellationToken).ConfigureAwait(true);
			if (response == null || response.StatusCode != HttpStatusCode.OK) return;
			using var stream = await response.Content.ReadAsStreamAsync(
#if NET5_0_OR_GREATER
				cancellationToken
#endif
			).ConfigureAwait(true);
			var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.FeatureCollectionGeoNetQuakeHistory, cancellationToken).ConfigureAwait(true);
			if (e == null || e.Features.Length == 0) return;

			if (!_lastModified.TryGetValue(id, out var lastModified)) {
				lastModified = DateTimeOffset.MinValue;
			}
			foreach (var feature in e.Features.TakeWhile(f => f.Properties.ModificationTime > lastModified).Reverse()) {
				Received?.Invoke(this, feature);
			}
			_lastModified[id] = e.Features[0].Properties.ModificationTime;
		}

		readonly Dictionary<string, ProgressiveDelay> _strongInfoDelay = [];
		readonly Dictionary<string, string> _strongInfoVersions = [];

		protected override async Task AfterHandled(CancellationToken cancellationToken) {
			if (!DoGetStrongMotionInfo) return;
			foreach (var delay in _strongInfoDelay) {
				if (!delay.Value.Step()) continue;
				await HandleStrongInfo(delay.Key, delay.Value, cancellationToken).ConfigureAwait(true);
			}
		}

		async Task HandleStrongInfo(string id, ProgressiveDelay delay, CancellationToken cancellationToken) {
			using var response = await TryGetAsync(new(_strongBaseUri, id), cancellationToken).ConfigureAwait(true);
			if (response == null || response.StatusCode != HttpStatusCode.OK) return;
			using var stream = await response.Content.ReadAsStreamAsync(
#if NET5_0_OR_GREATER
				cancellationToken
#endif
			).ConfigureAwait(true);
			var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.GeoNetStrong, cancellationToken).ConfigureAwait(true);
			if (e == null) return;
			if (_strongInfoVersions.TryGetValue(id, out var lastVersion) && e.Metadata.Version == lastVersion) return;
			delay.Reset();
			_strongInfoVersions[id] = e.Metadata.Version;
			Received?.Invoke(this, e);
		}

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);
	}
}
