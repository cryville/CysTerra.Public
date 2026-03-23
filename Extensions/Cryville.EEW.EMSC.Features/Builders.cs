using Cryville.EEW.Features;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.EMSC.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class EMSCRealTimeEventFeatureGeneratorBuilder : SimpleBuilder<EMSCRealTimeEventFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
