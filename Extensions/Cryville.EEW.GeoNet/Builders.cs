using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.GeoNet {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class GeoNetWorkerBuilder : IBuilder<GeoNetWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
		public GeoNetWorker Build(ref CultureInfo? culture) => new(
			new("https://api.geonet.org.nz/quake"),
			new("https://api.geonet.org.nz/quake/history/index"),
			new("https://api.geonet.org.nz/intensity/strong/processed/index")
		);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class GeoNetQuakeReportGeneratorBuilder : SimpleBuilder<GeoNetQuakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("Quake", ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class GeoNetQuakeHistoryReportGeneratorBuilder : SimpleBuilder<GeoNetQuakeHistoryReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("QuakeHistory", ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class GeoNetStrongReportGeneratorBuilder : SimpleBuilder<GeoNetStrongReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("Strong", ref culture);
	}
}
