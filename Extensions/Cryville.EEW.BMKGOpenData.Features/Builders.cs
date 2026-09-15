using Cryville.EEW.Features;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.BMKGOpenData.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class BMKGEarthquakeFeatureGeneratorBuilder : SimpleBuilder<BMKGEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
