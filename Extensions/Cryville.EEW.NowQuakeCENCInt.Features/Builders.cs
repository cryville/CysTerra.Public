using Cryville.EEW.Features;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.NowQuakeCENCInt.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class NowQuakeCENCIntDataFeatureGeneratorBuilder : SimpleBuilder<NowQuakeCENCIntDataFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
