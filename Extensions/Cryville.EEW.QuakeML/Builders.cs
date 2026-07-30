using Cryville.EEW.Report;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.QuakeML {
	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class QuakeMLEventReportGeneratorBuilder : SimpleBuilder<QuakeMLEventReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
