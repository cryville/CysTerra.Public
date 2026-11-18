using Cryville.EEW.TTS;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.NowQuakeCENCInt.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class NowQuakeCENCIntV2TTSMessageGeneratorBuilder : SimpleBuilder<NowQuakeCENCIntV2TTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceNameV2(ref culture);
	}

	[Obsolete("Use the V2 protocol instead.")]
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class NowQuakeCENCIntDataTTSMessageGeneratorBuilder : SimpleBuilder<NowQuakeCENCIntDataTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
