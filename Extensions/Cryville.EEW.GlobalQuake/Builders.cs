using Cryville.Common.Compat;
using Cryville.EEW.ComponentModel;
using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.GlobalQuake {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class GlobalQuakeWorkerBuilder : IBuilder<GlobalQuakeWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		[LocalizableDisplayName("PNHost")]
		public string? Host { get; set; }

		[LocalizableDisplayName("PNPort")]
		public int Port { get; set; } = 38000;

		public GlobalQuakeWorker Build(ref CultureInfo? culture) {
			ThrowHelper.ThrowIfNull(Host);
			return new GlobalQuakeWorker(Host, Port);
		}
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class GlobalQuakeReportGeneratorBuilder : SimpleBuilder<GlobalQuakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("SourceName");
		}
	}
}
