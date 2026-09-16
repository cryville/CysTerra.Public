using Cryville.EEW.Features;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.GeoNet.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class GeoNetQuakeFeatureGeneratorBuilder : SimpleBuilder<GeoNetQuakeFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("Quake", ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class GeoNetQuakeHistoryFeatureGeneratorBuilder : SimpleBuilder<GeoNetQuakeHistoryFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("QuakeHistory", ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class GeoNetStrongFeatureGeneratorBuilder : SimpleBuilder<GeoNetStrongFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("Strong", ref culture);
	}
}
