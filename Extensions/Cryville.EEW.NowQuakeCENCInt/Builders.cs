using Cryville.EEW.Report;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.NowQuakeCENCInt {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class NowQuakeCENCIntWorkerV2Builder : IBuilder<NowQuakeCENCIntWorkerV2> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceNameV2(ref culture);
		public NowQuakeCENCIntWorkerV2 Build(ref CultureInfo? culture) => new(new("wss://api-cencint-public.nowquake.cn/v2/websocket"));
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class NowQuakeCENCIntV2ReportGeneratorBuilder : SimpleBuilder<NowQuakeCENCIntV2ReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceNameV2(ref culture);
	}

	[Obsolete("Use the V2 protocol instead.")]
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class NowQuakeCENCIntWorkerBuilder : IBuilder<NowQuakeCENCIntWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
		public NowQuakeCENCIntWorker Build(ref CultureInfo? culture) => new(new("wss://api-cencint-public.nowquake.cn/websocket"));
	}

	[Obsolete("Use the V2 protocol instead.")]
	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class NowQuakeCENCIntDataReportGeneratorBuilder : SimpleBuilder<NowQuakeCENCIntDataReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
