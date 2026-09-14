using Cryville.EEW.ComponentModel;
using Cryville.EEW.CWAOpenData.Model;
using Cryville.EEW.Report;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.CWAOpenData {
	public enum CWAEarthquakeReportSubtype {
		[LocalizableDisplayName(nameof(Significant), Path = ["EarthquakeReportSubtype"])] Significant = 0,
		[LocalizableDisplayName(nameof(Minor), Path = ["EarthquakeReportSubtype"])] Minor = 1,
	}
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class CWAEarthquakeReportWorkerBuilder : IBuilder<CWAReportWorker<Earthquake>> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(Earthquake), ref culture);

		[LocalizableDisplayName("PNSubtype")]
		public CWAEarthquakeReportSubtype Subtype { get; set; }

		[LocalizableDisplayName("PNToken")]
		public string? Token { get; set; }

		public CWAReportWorker<Earthquake> Build(ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return new(new(Subtype switch {
				CWAEarthquakeReportSubtype.Significant => "https://opendata.cwa.gov.tw/api/v1/rest/datastore/E-A0015-001",
				CWAEarthquakeReportSubtype.Minor => "https://opendata.cwa.gov.tw/api/v1/rest/datastore/E-A0016-001",
				_ => throw new ArgumentException(res.GetStringRequired("ErrorUnknownSubtype")),
			}), Token ?? throw new ArgumentNullException(res.GetStringRequired("ErrorUnauthorized")));
		}
	}

	[Export(typeof(IBuilder<ISourceWorker>))]
	public class CWATsunamiReportWorkerBuilder : IBuilder<CWAReportWorker<Tsunami>> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(Tsunami), ref culture);

		[LocalizableDisplayName("PNToken")]
		public string? Token { get; set; }

		public CWAReportWorker<Tsunami> Build(ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return new(new(
				"https://opendata.cwa.gov.tw/api/v1/rest/datastore/E-A0014-001"),
				Token ?? throw new ArgumentNullException(res.GetStringRequired("ErrorUnauthorized")),
				1440, 17280
			);
		}
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CWAEarthquakeReportGeneratorBuilder : SimpleBuilder<CWAEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(Earthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CWATsunamiReportGeneratorBuilder : SimpleBuilder<CWATsunamiReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(Tsunami), ref culture);
	}
}
