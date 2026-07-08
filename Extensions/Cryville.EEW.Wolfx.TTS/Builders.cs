using Cryville.EEW.TTS;
using Cryville.EEW.Wolfx.Model;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.Wolfx.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class CENCEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<CENCEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CENCEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class CENCEEWTTSMessageGeneratorBuilder : SimpleBuilder<CENCEEWTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CENCEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class CWAEEWTTSMessageGeneratorBuilder : SimpleBuilder<CWAEEWTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CWAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class FujianEEWTTSMessageGeneratorBuilder : SimpleBuilder<FujianEEWTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(FujianEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class JMAEEWTTSMessageGeneratorBuilder : SimpleBuilder<JMAEEWTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(JMAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class SichuanEEWTTSMessageGeneratorBuilder : SimpleBuilder<SichuanEEWTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(SichuanEEW), ref culture);
	}
}
