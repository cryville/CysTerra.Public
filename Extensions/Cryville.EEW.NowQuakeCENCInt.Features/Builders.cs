using Cryville.EEW.Features;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.NowQuakeCENCInt.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class NowQuakeCENCIntV2FeatureGeneratorBuilder : SimpleBuilder<NowQuakeCENCIntV2FeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceNameV2(ref culture);
	}

	[Obsolete("Use the V2 protocol instead.")]
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class NowQuakeCENCIntDataFeatureGeneratorBuilder : SimpleBuilder<NowQuakeCENCIntDataFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
