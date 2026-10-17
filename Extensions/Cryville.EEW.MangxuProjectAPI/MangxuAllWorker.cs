using Cryville.EEW.ComponentModel;
using Cryville.EEW.MangxuProjectAPI.Model;
using Cryville.EEW.USGS;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.MangxuProjectAPI {
	public class MangxuAllWorker : WebSocketWorker, ISourceWorker, IPropertiesHolder, IUSGSGeoJSONFeedWorker {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public event Handler<object?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);

		readonly USGSGeoJSONFeedHandler _usgsGeoJSONFeedHandler;
		public MangxuAllWorker(Uri uri) : base(uri) {
			m_uri = InitialUri;
			_usgsGeoJSONFeedHandler = new(this);
			_internalHttpWorker = new(this);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (!disposing)
				return;
			_internalHttpWorker.Dispose();
		}

		bool m_doFilterByMinMagnitude;
		[LocalizableDisplayName("PNDoFilterByMinMagnitude")]
		public bool DoFilterByMinMagnitude {
			get => m_doFilterByMinMagnitude;
			set {
				if (m_doFilterByMinMagnitude == value)
					return;
				m_doFilterByMinMagnitude = value;
				UpdateSubscription();
			}
		}

		float m_minMagnitude;
		[LocalizableDisplayName("PNMinMagnitude")]
		public float MinMagnitude {
			get => m_minMagnitude;
			set {
				if (m_minMagnitude == value)
					return;
				m_minMagnitude = value;
				if (DoFilterByMinMagnitude) {
					UpdateSubscription();
				}
			}
		}

		bool m_doFilterByAgency;
		[LocalizableDisplayName("PNDoFilterByAgency")]
		[LocalizableDescription("PDDoFilterByAgency")]
		public bool DoFilterByAgency {
			get => m_doFilterByAgency;
			set {
				if (m_doFilterByAgency == value)
					return;
				m_doFilterByAgency = value;
				UpdateSubscription();
			}
		}

		ISet<MangxuAgency> m_filter = (HashSet<MangxuAgency>)[];
		[LocalizableDisplayName("PNAgencies")]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		public ISet<MangxuAgency> Agencies {
			get => m_filter;
			set {
				if (m_filter.SequenceEqual(value))
					return;
				m_filter = value;
				if (DoFilterByAgency) {
					UpdateSubscription();
				}
			}
		}

		[LocalizableDisplayName("PNUsgsFilter")]
		[LocalizableDescription("PDUsgsFilter")]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		public ISet<USGSProductType> UsgsFilter {
			get => _usgsGeoJSONFeedHandler.Filter;
			set => _usgsGeoJSONFeedHandler.Filter = value;
		}

		Uri m_uri;
		protected override Uri GetUri() => m_uri;
		void UpdateSubscription() {
			var builder = new UriBuilder(InitialUri);
			var queryParams = new Dictionary<string, string>();
			if (DoFilterByMinMagnitude)
				queryParams.Add("min_mag", MinMagnitude.ToString(CultureInfo.InvariantCulture));
			List<string>? agencies = null;
			if (DoFilterByAgency) {
				agencies = [.. Agencies.Select(a => ((ILocalizableMetadataAttribute?)a.GetType().GetField(a.ToString())?.GetCustomAttribute<LocalizableDisplayNameAttribute>() ?? throw new InvalidOperationException("Invalid agency.")).Name)];
				queryParams.Add("agencies", string.Join(',', agencies));
			}
			builder.Query = string.Join('&', queryParams.Select(p => string.Format(CultureInfo.InvariantCulture, "{0}={1}", p.Key, Uri.EscapeDataString(p.Value))));
			m_uri = builder.Uri;

			if (!_isConnected)
				return;
			Task.Run(async () => {
				using var stream = new MemoryStream();
				await JsonSerializer.SerializeAsync(stream, new SubscribeMessage(
					DoFilterByMinMagnitude ? MinMagnitude : null,
					agencies
				), SerializerContext.Default.ClientMessage).ConfigureAwait(true);
				stream.Position = 0;
				try {
					await SendAsync(stream, WebSocketMessageType.Text, CancellationToken.None).ConfigureAwait(true);
				}
				catch (ObjectDisposedException) { }
			});
		}

		bool _isConnected;
		protected override Task OnConnected(CancellationToken cancellationToken) {
			_isConnected = true;
			return base.OnConnected(cancellationToken);
		}
		protected override async Task Handle(Stream stream, WebSocketMessageType messageType, CancellationToken cancellationToken) {
			// TODO: Restore events from full_snapshot
			var msg = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.Message, cancellationToken).ConfigureAwait(true)
				?? throw new JsonException("Null event.");
			if (msg is SingleAgencyUpdateMessage singleAgencyUpdateMessage) {
				foreach (var e in singleAgencyUpdateMessage.Events) {
					await HandleEvent(e, cancellationToken).ConfigureAwait(true);
				}
			}
			else if (msg is BatchAgencyUpdateMessage batchAgencyUpdateMessage) {
				foreach (var update in batchAgencyUpdateMessage.Updates) {
					foreach (var e in update.Events) {
						await HandleEvent(e, cancellationToken).ConfigureAwait(true);
					}
				}
			}
		}
		const int MAX_USGS_HISTORY_COUNT = 256;
		readonly Queue<string> _usgsHistory = new(MAX_USGS_HISTORY_COUNT);
		async Task HandleEvent(EarthquakeEvent e, CancellationToken cancellationToken) {
			if (e.SaRaw is { } saRaw) {
				string id = saRaw.Network + saRaw.Code;
				if (_usgsHistory.Count >= MAX_USGS_HISTORY_COUNT) {
					string dequeued = _usgsHistory.Dequeue();
					if (!_usgsHistory.Contains(dequeued)) {
						_usgsGeoJSONFeedHandler.RemoveHistory(dequeued);
					}
				}
				_usgsHistory.Enqueue(id);
				await _usgsGeoJSONFeedHandler.Handle(saRaw, cancellationToken).ConfigureAwait(true);
			}
			else if (e.CeaData is { } ceaData) {
				Received?.Invoke(this, ceaData);
			}
			else if (e.EewRaw is { } eewRaw) {
				Received?.Invoke(this, eewRaw);
			}
			else if (e.CwaEewRaw is { } cwaEewRaw) {
				Received?.Invoke(this, cwaEewRaw);
			}
			else if (e.CeicRaw is { } ceicRaw) {
				Received?.Invoke(this, ceicRaw);
			}
			else if (e.P2pRaw is { } p2pRaw) {
				Received?.Invoke(this, p2pRaw);
			}
			else {
				Received?.Invoke(this, e);
			}
		}

		readonly InternalHttpWorker _internalHttpWorker;
		sealed class InternalHttpWorker(MangxuAllWorker parentWorker) : HttpPullWorker(new("http://invalid/")) {
			protected override Task Handle(Stream stream, HttpResponseHeaders headers, CancellationToken cancellationToken) => throw new NotSupportedException();
			protected override void OnError(Exception ex) => parentWorker.OnError(ex);
			protected override void OnHeartbeat() => throw new NotSupportedException();

			public Task<HttpResponseMessage?> TryGetAsync(Uri uri, CancellationToken cancellationToken) => TryGetAsync(uri, cancellationToken, HttpRetryStrategies.Report | HttpRetryStrategies.RetryBeforeExit);
		}
		public Task<HttpResponseMessage?> TryGetAsync(Uri uri, CancellationToken cancellationToken) => _internalHttpWorker.TryGetAsync(uri, cancellationToken);
		public void OnReceived(object e) => Received?.Invoke(this, e);
		void IUSGSGeoJSONFeedWorker.OnError(Exception ex) => OnError(ex);
	}
}
