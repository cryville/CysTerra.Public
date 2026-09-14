using Cryville.EEW.CWAOpenData.Model;
using Cryville.EEW.Features;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.CWAOpenData.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class CWAEarthquakeFeatureGeneratorBuilder : SimpleBuilder<CWAEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(Earthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class CWATsunamiFeatureGeneratorBuilder : SimpleBuilder<CWATsunamiFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(Tsunami), ref culture);
	}
}
