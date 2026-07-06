using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.TTS;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.FANStudio.TTS {
	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class CENCEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<CENCEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CENCEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class CEAEEWTTSMessageGeneratorBuilder : SimpleBuilder<CEAEEWTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CEAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class NingxiaEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<NingxiaEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(NingxiaEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class GuangxiEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<GuangxiEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(GuangxiEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class ShanxiEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<ShanxiEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(ShanxiEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class BeijingEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<BeijingEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(BeijingEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class CWAEEWTTSMessageGeneratorBuilder : SimpleBuilder<CWAEEWTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CWAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class HKOEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<HKOEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(HKOEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class ShakeAlertEEWTTSMessageGeneratorBuilder : SimpleBuilder<ShakeAlertEEWTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(ShakeAlertEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class KMAEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<KMAEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(KMAEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<TTSEntry>>))]
	public class FSSNEarthquakeTTSMessageGeneratorBuilder : SimpleBuilder<FSSNEarthquakeTTSMessageGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(FSSNEarthquake), ref culture);
	}
}
