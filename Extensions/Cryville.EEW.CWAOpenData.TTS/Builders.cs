using Cryville.EEW.CWAOpenData.Model;
using Cryville.EEW.TTS;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.CWAOpenData.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class CWAEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<CWAEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(Earthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class CWATsunamiTTSMessageGeneratorBuilder : SimpleBuilder<CWATsunamiTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(Tsunami), ref culture);
	}
}
