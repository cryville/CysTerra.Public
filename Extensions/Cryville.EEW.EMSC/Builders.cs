using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.EMSC {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class EMSCRealTimeWorkerBuilder : IBuilder<EMSCRealTimeWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
		public EMSCRealTimeWorker Build(ref CultureInfo? culture) => new(new("wss://www.seismicportal.eu/standing_order/websocket"));
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class EMSCRealTimeEventReportGeneratorBuilder : SimpleBuilder<EMSCRealTimeEventReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
