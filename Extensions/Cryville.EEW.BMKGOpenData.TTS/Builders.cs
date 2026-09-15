using Cryville.EEW.TTS;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.BMKGOpenData.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class BMKGEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<BMKGEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
