using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.BMKGOpenData {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class BMKGOpenDataWorkerBuilder : IBuilder<BMKGOpenDataWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
		public BMKGOpenDataWorker Build(ref CultureInfo? culture) => new(new("https://data.bmkg.go.id/DataMKG/TEWS/autogempa.json"));
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class BMKGEarthquakeReportGeneratorBuilder : SimpleBuilder<BMKGEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
