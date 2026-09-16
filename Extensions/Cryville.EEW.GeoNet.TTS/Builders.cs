using Cryville.EEW.TTS;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.GeoNet.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class GeoNetQuakeTTSMessageGeneratorBuilder : SimpleBuilder<GeoNetQuakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("Quake", ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class GeoNetQuakeHistoryTTSMessageGeneratorBuilder : SimpleBuilder<GeoNetQuakeHistoryTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("QuakeHistory", ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class GeoNetStrongTTSMessageGeneratorBuilder : SimpleBuilder<GeoNetStrongTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("Strong", ref culture);
	}
}
