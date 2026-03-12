using Cryville.EEW.Features;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.GlobalQuake.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class GlobalQuakeReportFeatureGeneratorBuilder : SimpleBuilder<GlobalQuakeReportFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
