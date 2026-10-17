using Cryville.EEW.MangxuProjectAPI.Model;
using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.MangxuProjectAPI {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class MangxuAllWorkerBuilder : IBuilder<MangxuAllWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
		public MangxuAllWorker Build(ref CultureInfo? culture) => new(new("wss://ws.mangxufurry.cc.cd/ws/agency/all"));
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class EarthquakeEventReportGeneratorBuilder : SimpleBuilder<EarthquakeEventReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("", ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CeaDataReportGeneratorBuilder : SimpleBuilder<CeaDataReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("cea", ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CeicRawReportGeneratorBuilder : SimpleBuilder<CeicRawReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("ceic", ref culture);
	}
}
