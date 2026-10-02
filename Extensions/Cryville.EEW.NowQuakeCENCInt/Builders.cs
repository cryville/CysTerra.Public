using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.NowQuakeCENCInt {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class NowQuakeCENCIntWorkerBuilder : IBuilder<NowQuakeCENCIntWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
		public NowQuakeCENCIntWorker Build(ref CultureInfo? culture) => new(new("wss://api-cencint-public.nowquake.cn/websocket"));
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class NowQuakeCENCIntDataReportGeneratorBuilder : SimpleBuilder<NowQuakeCENCIntDataReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
