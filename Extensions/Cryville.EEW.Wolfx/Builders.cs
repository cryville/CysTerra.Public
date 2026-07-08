using Cryville.EEW.Report;
using Cryville.EEW.Wolfx.Model;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.Wolfx {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class WolfxWorkerBuilder : IBuilder<WolfxWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
		public WolfxWorker Build(ref CultureInfo? culture) => new(new("wss://ws-api.wolfx.jp/all_eew"));
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CENCEarthquakeReportGeneratorBuilder : SimpleBuilder<CENCEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CENCEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CENCEEWReportGeneratorBuilder : SimpleBuilder<CENCEEWReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CENCEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CWAEEWReportGeneratorBuilder : SimpleBuilder<CWAEEWReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CWAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class FujianEEWReportGeneratorBuilder : SimpleBuilder<FujianEEWReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(FujianEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class JMAEEWReportGeneratorBuilder : SimpleBuilder<JMAEEWReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(JMAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class SichuanEEWReportGeneratorBuilder : SimpleBuilder<SichuanEEWReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(SichuanEEW), ref culture);
	}
}
