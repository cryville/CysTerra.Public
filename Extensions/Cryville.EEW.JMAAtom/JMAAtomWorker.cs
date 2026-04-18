using Cryville.EEW.Atom;
using Cryville.EEW.ComponentModel;
using Cryville.EEW.JMAAtom.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Cryville.EEW.JMAAtom {
	public class JMAAtomWorker : HttpPullWorker, ISourceWorker<JMAReport>, IPropertiesHolder {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		public event Handler<JMAReport?>? Received;
		public event Handler<Heartbeat>? Heartbeat;
		public event Handler<Exception>? ErrorEmitted;

		protected override void OnHeartbeat() => Heartbeat?.Invoke(this, EEW.Heartbeat.Instance);
		protected override void OnError(Exception ex) => ErrorEmitted?.Invoke(this, ex);

		[LocalizableDisplayName("PNIsFilterWhitelist")]
		[LocalizableDescription("PDIsFilterWhitelist")]
		public bool IsFilterWhitelist { get; set; }

		static readonly Dictionary<JMAAtomInfoCode, string> _filterMap = new() {
			{ JMAAtomInfoCode.VFVO50, "噴火警報・予報" },
			{ JMAAtomInfoCode.VFVO51, "火山の状況に関する解説情報" },
			{ JMAAtomInfoCode.VFVO52, "噴火に関する火山観測報" },
			{ JMAAtomInfoCode.VFVO53, "降灰予報（定時）" },
			{ JMAAtomInfoCode.VFVO54, "降灰予報（速報）" },
			{ JMAAtomInfoCode.VFVO55, "降灰予報（詳細）" },
			{ JMAAtomInfoCode.VFVO56, "噴火速報" },
			{ JMAAtomInfoCode.VFVO60, "推定噴煙流向報" },
			{ JMAAtomInfoCode.VTSE41, "津波警報・注意報・予報a" },
			{ JMAAtomInfoCode.VTSE51, "津波情報a" },
			{ JMAAtomInfoCode.VTSE52, "沖合の津波観測に関する情報" },
			{ JMAAtomInfoCode.VXSE51, "震度速報" },
			{ JMAAtomInfoCode.VXSE52, "震源に関する情報" },
			{ JMAAtomInfoCode.VXSE53, "震源・震度に関する情報" },
			{ JMAAtomInfoCode.VXSE56, "地震の活動状況等に関する情報" },
			{ JMAAtomInfoCode.VXSE60, "地震回数に関する情報" },
			{ JMAAtomInfoCode.VXSE61, "顕著な地震の震源要素更新のお知らせ" },
			{ JMAAtomInfoCode.VXSE62, "長周期地震動に関する観測情報" },
			{ JMAAtomInfoCode.VYSE50, "南海トラフ地震臨時情報" },
			{ JMAAtomInfoCode.VYSE51VYSE52, "南海トラフ地震関連解説情報" },
		};
		[LocalizableDisplayName("PNFilter")]
		[SuppressMessage("Usage", "CA2227", Justification = "Component property")]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		public ISet<JMAAtomInfoCode> Filter {
			get => m_filter;
			set {
				m_filter = value;
				MapFilter();
			}
		}
		[MemberNotNull(nameof(_titleFilter))]
		void MapFilter() {
			_titleFilter = [.. m_filter.Select(i => _filterMap.TryGetValue(i, out string? title) ? title : "")];
		}
		ISet<JMAAtomInfoCode> m_filter = (HashSet<JMAAtomInfoCode>)[JMAAtomInfoCode.VFVO53, JMAAtomInfoCode.VFVO60];
		HashSet<string> _titleFilter;
		[Obsolete("Use the Filter property.")]
		public void SetFilter(IEnumerable<string> filter) => _titleFilter = [.. filter];

		public JMAAtomWorker(Uri uri) : base(uri) {
			MapFilter();
		}

#if NET5_0_OR_GREATER
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AtomFeed))]
		[UnconditionalSuppressMessage("Trimming", "IL2026")]
#endif
		readonly XmlSerializer _xmlSerializer = new(typeof(AtomFeed));
#if NET5_0_OR_GREATER
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JMAReport))]
		[UnconditionalSuppressMessage("Trimming", "IL2026")]
#endif
		readonly XmlSerializer _reportXmlSerializer = new(typeof(JMAReport));

		bool _init;
		readonly HashSet<string> _history = [];
#if NET5_0_OR_GREATER
		[UnconditionalSuppressMessage("Trimming", "IL2026")]
#endif
		protected override async Task Handle(Stream stream, HttpResponseHeaders headers, CancellationToken cancellationToken) {
			try {
				using var reader = XmlReader.Create(stream, SharedSettings.XmlReaderSettings);
				var atom = (AtomFeed)(_xmlSerializer.Deserialize(reader) ?? throw new InvalidOperationException("Empty atom."));
				if (_init) {
					foreach (var entry in atom.Entries.Reverse()) {
						if (!_history.Add(entry.Id)) continue;
						if (_titleFilter.Contains(entry.Title) != IsFilterWhitelist) continue;
						await ParseEntry(entry, cancellationToken).ConfigureAwait(true);
					}
					_history.RemoveWhere(e => !atom.Entries.Any(r => r.Id == e));
				}
				else {
					_init = true;
#if DEBUG
					foreach (var report in atom.Entries.Reverse()) {
						await ParseEntry(report, cancellationToken).ConfigureAwait(true);
					}
#endif
					foreach (var entry in atom.Entries) {
						_history.Add(entry.Id);
					}
				}
			}
			catch (InvalidOperationException ex) when (ex.InnerException is XmlException) {
				OnError(new SourceWorkerNetworkException("Server returned invalid data.", ex.InnerException));
			}
		}

#if NET5_0_OR_GREATER
		[UnconditionalSuppressMessage("Trimming", "IL2026")]
#endif
		internal async Task ParseEntry(AtomEntry entry, CancellationToken cancellationToken) {
			foreach (var link in entry.Links) {
				try {
					using var response = await TryGetAsync(new Uri(link.HRef), cancellationToken).ConfigureAwait(true);
					if (response == null || response.StatusCode != HttpStatusCode.OK) continue;
					cancellationToken.ThrowIfCancellationRequested();
					using var stream = await response.Content.ReadAsStreamAsync(
#if NET5_0_OR_GREATER
						cancellationToken
#endif
					).ConfigureAwait(true);
					cancellationToken.ThrowIfCancellationRequested();
					using var reader = XmlReader.Create(stream, SharedSettings.XmlReaderSettings);
					Received?.Invoke(this, (JMAReport?)_reportXmlSerializer.Deserialize(reader));
					return;
				}
				catch (InvalidOperationException ex) when (ex.InnerException is XmlException) {
					OnError(ex.InnerException);
				}
			}
		}
	}
}
