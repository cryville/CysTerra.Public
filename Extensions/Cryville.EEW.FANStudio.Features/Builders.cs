using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.FANStudio.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class BeijingEarthquakeFeatureGeneratorBuilder : SimpleBuilder<BeijingEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(BeijingEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class CEAEEWFeatureGeneratorBuilder : SimpleBuilder<CEAEEWFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CEAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class CENCEarthquakeFeatureGeneratorBuilder : SimpleBuilder<CENCEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CENCEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class CWAEEWFeatureGeneratorBuilder : SimpleBuilder<CWAEEWFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CWAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class FSSNEarthquakeFeatureGeneratorBuilder : SimpleBuilder<FSSNEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(FSSNEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class EMSCEarthquakeFeatureGeneratorBuilder : SimpleBuilder<EMSCEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(EMSCEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class BCSFEarthquakeFeatureGeneratorBuilder : SimpleBuilder<BCSFEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(BCSFEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class GFZEarthquakeFeatureGeneratorBuilder : SimpleBuilder<GFZEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(GFZEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class USPEarthquakeFeatureGeneratorBuilder : SimpleBuilder<USPEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(USPEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class GuangxiEarthquakeFeatureGeneratorBuilder : SimpleBuilder<GuangxiEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(GuangxiEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class HKOEarthquakeFeatureGeneratorBuilder : SimpleBuilder<HKOEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(HKOEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class KMAEarthquakeFeatureGeneratorBuilder : SimpleBuilder<KMAEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(KMAEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class NingxiaEarthquakeFeatureGeneratorBuilder : SimpleBuilder<NingxiaEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(NingxiaEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class ShakeAlertEEWFeatureGeneratorBuilder : SimpleBuilder<ShakeAlertEEWFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(ShakeAlertEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class ShanxiEarthquakeFeatureGeneratorBuilder : SimpleBuilder<ShanxiEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(ShanxiEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class YunnanEarthquakeFeatureGeneratorBuilder : SimpleBuilder<YunnanEarthquakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(YunnanEarthquake), ref culture);
	}
}
