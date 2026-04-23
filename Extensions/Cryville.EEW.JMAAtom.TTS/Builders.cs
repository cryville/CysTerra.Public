using Cryville.EEW.TTS;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.JMAAtom.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class JMAAtomTTSMessageGeneratorBuilder : SimpleBuilder<JMAAtomTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
