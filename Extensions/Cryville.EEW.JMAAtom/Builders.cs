using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.JMAAtom {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class JMAAtomWorkerBuilder : IBuilder<JMAAtomWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
		public JMAAtomWorker Build(ref CultureInfo? culture) => new(new("https://www.data.jma.go.jp/developer/xml/feed/eqvol.xml"));
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class JMAAtomReportGeneratorBuilder : SimpleBuilder<JMAAtomReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
