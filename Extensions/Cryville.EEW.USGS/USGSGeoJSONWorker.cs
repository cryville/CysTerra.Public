using Cryville.EEW.ComponentModel;
using Cryville.EEW.USGS.Model;
using QuakeML;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Cryville.EEW.USGS {
	public class USGSGeoJSONWorker : USGSWorker {
		public USGSGeoJSONWorker(Uri uri) : base(uri, ".geojson") {
			_fileHandlers = new() {
				{ "origin", new() {
					{ "quakeml.xml", ParseOrigin },
				} },
				{ "shakemap", new() {
					{ "download/cont_mi.json", ParseShakeMapContours },
					{ "download/cont_mmi.json", ParseShakeMapContours },
					{ "download/cont_pga.json", ParseShakeMapContours },
					{ "download/cont_pgv.json", ParseShakeMapContours },
				} },
			};
			Filter = (HashSet<USGSProductType>)[];
		}

		public override event Handler<object?>? Received;

		ISet<USGSProductType> m_filter;
		[LocalizableDisplayName("PNFilter")]
		[LocalizableDescription("PDFilter")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		public ISet<USGSProductType> Filter {
			get => m_filter;
			[MemberNotNull(nameof(m_filter))]
			set {
				m_filter = value;
				_pathFilter = m_filter.Select(p => p switch {
					USGSProductType.Origin => ("origin", "quakeml.xml"),
					USGSProductType.ShakeMapContMmi => ("shakemap", "download/cont_mmi.json"),
					USGSProductType.ShakeMapContPga => ("shakemap", "download/cont_pga.json"),
					USGSProductType.ShakeMapContPgv => ("shakemap", "download/cont_pgv.json"),
					_ => ("", ""),
				})
				.GroupBy(i => i.Item1)
				.ToDictionary(g => g.Key, g => g.Select(i => i.Item2).ToHashSet());
			}
		}
		Dictionary<string, HashSet<string>> _pathFilter = [];
		[Obsolete("Use the Filter property.")]
		public void SetFilter(IReadOnlyDictionary<string, IEnumerable<string>> filter) {
			_pathFilter = filter.ToDictionary(i => i.Key, i => i.Value.ToHashSet());
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
				await HandleDetail(props, cancellationToken).ConfigureAwait(true);
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
				_detailHistory.Remove(id);
			}
			_init = true;
		}

		readonly Dictionary<string, Dictionary<string, Dictionary<string, ProductHistory>>> _detailHistory = [];
		sealed class ProductHistory(long updateTimestamp) {
			public long _lastUpdateTimestamp = updateTimestamp;
			public readonly Dictionary<string, long> _fileTimestamps = [];
		}
		async Task HandleDetail(USGSEarthquakeSummary summary, CancellationToken cancellationToken) {
			using var response = await TryGetAsync(summary.DetailUrl, cancellationToken, HttpRetryStrategies.Report | HttpRetryStrategies.RetryBeforeExit).ConfigureAwait(true);
			if (response == null || response.StatusCode != HttpStatusCode.OK) return;
			using var stream = await response.Content.ReadAsStreamAsync(
#if NET5_0_OR_GREATER
				cancellationToken
#endif
			).ConfigureAwait(true);
			var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.FeatureUSGSEarthquakeDetail, cancellationToken).ConfigureAwait(true);
			if (e == null) return;

			string id = e.Id.GetString() ?? throw new FormatException("Missing event ID.");
			if (!_detailHistory.TryGetValue(id, out var history))
				_detailHistory.Add(id, history = []);
			foreach (var type in _pathFilter) {
				string typeName = type.Key;
				if (!e.Properties.Products.TryGetValue(typeName, out var products)) continue;
				if (!history.TryGetValue(typeName, out var history2)) {
					history.Add(typeName, history2 = []);
				}
				foreach (var product in products) {
					string productCode = product.Code;
					long updateTimestamp = product.UpdateTimestamp;
					if (history2.TryGetValue(productCode, out var productHistory)) {
						if (productHistory._lastUpdateTimestamp == updateTimestamp) continue;
						productHistory._lastUpdateTimestamp = updateTimestamp;
					}
					else {
						history2.Add(productCode, productHistory = new(updateTimestamp));
					}
					var fileHistory = productHistory._fileTimestamps;
					foreach (var file in type.Value) {
						if (product.Contents == null) continue;
						if (!product.Contents.TryGetValue(file, out var content)) continue;
						long fileTimestamp = content.LastModified;
						if (fileHistory.TryGetValue(file, out var lastFileTimestamp) && lastFileTimestamp == fileTimestamp) continue;
						fileHistory[file] = fileTimestamp;
						if (!_fileHandlers.TryGetValue(typeName, out var handlers)) continue;
						if (!handlers.TryGetValue(file, out var handler)) continue;
						await HandleDetailContent(content, file, product, handler, cancellationToken).ConfigureAwait(true);
					}
				}
			}
		}

		async Task HandleDetailContent(USGSProductContent content, string fileName, USGSEarthquakeProduct product, FileHandler handler, CancellationToken cancellationToken) {
			if (content.Url is not Uri url) return;
			using var response = await TryGetAsync(url, cancellationToken, HttpRetryStrategies.Report | HttpRetryStrategies.RetryBeforeExit).ConfigureAwait(true);
			if (response == null || response.StatusCode != HttpStatusCode.OK) return;
			using var stream = await response.Content.ReadAsStreamAsync(
#if NET5_0_OR_GREATER
				cancellationToken
#endif
			).ConfigureAwait(true);
			var e = await handler(stream, product, fileName, content, cancellationToken).ConfigureAwait(true);
			if (e == null) return;
			Received?.Invoke(this, e);
		}

		delegate Task<object?> FileHandler(Stream stream, USGSEarthquakeProduct product, string fileName, USGSProductContent content, CancellationToken cancellationToken);
		readonly Dictionary<string, Dictionary<string, FileHandler>> _fileHandlers;

#if NET5_0_OR_GREATER
		[UnconditionalSuppressMessage("Trimming", "IL2026")]
#endif
		Task<object?> ParseOrigin(Stream stream, USGSEarthquakeProduct product, string fileName, USGSProductContent content, CancellationToken cancellationToken) {
			using var reader = XmlReader.Create(stream, SharedSettings.XmlReaderSettings);
			Quakeml? quakeml;
			try {
				quakeml = (Quakeml?)Shared.QuakeMLSerializer.Deserialize(reader);
			}
			catch (InvalidOperationException ex) {
				OnError(ex);
				return Task.FromResult<object?>(null);
			}
			if (quakeml == null)
				throw new InvalidOperationException("Empty QuakeML.");
			var e = quakeml.eventParameters.@event.Single();
			if (e == null) return Task.FromResult<object?>(null);
			return Task.FromResult<object?>(e);
		}

		async Task<object?> ParseShakeMapContours(Stream stream, USGSEarthquakeProduct product, string fileName, USGSProductContent content, CancellationToken cancellationToken) {
			try {
				var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.USGSContours, cancellationToken).ConfigureAwait(true);
				if (e == null) return null;
				return new USGSContoursProduct(e, product, fileName, content);
			}
			catch (JsonException ex) {
				OnError(ex);
				return null;
			}
		}
	}
}
