using Cryville.EEW.Features;
using Cryville.EEW.Wolfx.Model;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.Wolfx.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class CENCEarthquakeFeatureGeneratorBuilder : SimpleBuilder<CENCEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CENCEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class CENCEEWFeatureGeneratorBuilder : SimpleBuilder<CENCEEWFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CENCEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class CWAEEWFeatureGeneratorBuilder : SimpleBuilder<CWAEEWFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CWAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class FujianEEWFeatureGeneratorBuilder : SimpleBuilder<FujianEEWFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(FujianEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class JMAEEWFeatureGeneratorBuilder : SimpleBuilder<JMAEEWFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(JMAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class SichuanEEWFeatureGeneratorBuilder : SimpleBuilder<SichuanEEWFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(SichuanEEW), ref culture);
	}
}
