using Cryville.EEW.Features;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.JMAAtom.Features {
	[Export(typeof(IBuilder<IGenerator<Feature>>))]
	public class JMAAtomFeatureGeneratorBuilder : SimpleBuilder<JMAAtomFeatureGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
