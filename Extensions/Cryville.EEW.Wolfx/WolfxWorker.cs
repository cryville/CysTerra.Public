using Cryville.EEW.ComponentModel;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cryville.EEW.Wolfx {
	public class WolfxWorker : WebSocketWorker, ISourceWorker<BaseModel>, IPropertiesHolder {
		public string? GetName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("SourceName");
		}

		public event Handler<BaseModel?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);

		[Obsolete("Filter is always whitelist.")]
		[Browsable(false)]
		public bool IsFilterWhitelist { get; set; }

		static readonly Dictionary<WolfxSource, Type> _filterMap = new() {
			{ WolfxSource.SichuanEEW, typeof(SichuanEEW) },
			{ WolfxSource.JMAEEW, typeof(JMAEEW) },
			{ WolfxSource.FujianEEW, typeof(FujianEEW) },
			{ WolfxSource.CWAEEW, typeof(CWAEEW) },
			{ WolfxSource.ChongqingEEW, typeof(ChongqingEEW) },
			{ WolfxSource.CENCEEW, typeof(CENCEEW) },
			{ WolfxSource.CENCEarthquake, typeof(WolfxEarthquakeList<CENCEarthquake>) },
			{ WolfxSource.JMAEarthquake, typeof(WolfxEarthquakeList<JMAEarthquake>) },
		};
		[LocalizableDisplayName("PNFilter")]
		[LocalizableDescription("PDFilter")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		public ISet<WolfxSource> Filter {
			get => m_filter;
			set {
				m_filter = value;
				MapFilter();
			}
		}
		[MemberNotNull(nameof(_typeFilter))]
		void MapFilter() {
			_typeFilter = [.. m_filter.Select(i => _filterMap.TryGetValue(i, out var type) ? type : null).OfType<Type>()];
		}
		ISet<WolfxSource> m_filter = (HashSet<WolfxSource>)[WolfxSource.JMAEEW];
		HashSet<Type> _typeFilter = [];
		[Obsolete("Use the Filter property.")]
		public void SetFilter(IEnumerable<Type> filter) => _typeFilter = [.. filter];

		public WolfxWorker(Uri uri) : base(uri) {
			MapFilter();
		}

		protected override async Task Handle(Stream stream, WebSocketMessageType messageType, CancellationToken cancellationToken) {
			try {
				var e = await JsonSerializer.DeserializeAsync(stream, SerializerContext.Default.BaseModel, cancellationToken).ConfigureAwait(true) ?? throw new JsonException("Null event.");
				if (_typeFilter.Contains(e.GetType())) {
					Received?.Invoke(this, e);
				}
			}
			catch (JsonException ex) {
				ErrorEmitted?.Invoke(this, ex);
			}
		}
	}
}
