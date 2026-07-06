using Cryville.EEW.ComponentModel;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class FANStudioAllWorkerBuilder : IBuilder<FANStudioAllWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("$Multiple", ref culture);
		public FANStudioAllWorker Build(ref CultureInfo? culture) => new(new("wss://ws.fanstudio.tech/all"), FANStudioSourceTypeInfoProvider.Instance);
	}

	[Export(typeof(IBuilder<ISourceWorker>))]
	public class FANStudioWorkerBuilder : IBuilder<ISourceWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName("$Single", ref culture);

		[LocalizableDisplayName("PNSource")]
		public FANStudioSource Source { get; set; }

		public ISourceWorker Build(ref CultureInfo? culture) {
			return Source switch {
				FANStudioSource.CMAWeatherAlarm => Build<CMAWeatherAlarm>(),
				FANStudioSource.NMEFCTsunamiWarning => Build<NMEFCTsunamiWarning>(),
				FANStudioSource.CENCEarthquake => Build<CENCEarthquake>(),
				// FANStudioSource.CENCIntensityReport => Build<CENCIntensityReport>(),
				FANStudioSource.CEAEEW => Build<CEAEEW>(),
				FANStudioSource.CEAProvinceEEW => Build<CEAProvinceEEW>(),
				FANStudioSource.NingxiaEarthquake => Build<NingxiaEarthquake>(),
				FANStudioSource.GuangxiEarthquake => Build<GuangxiEarthquake>(),
				FANStudioSource.ShanxiEarthquake => Build<ShanxiEarthquake>(),
				FANStudioSource.BeijingEarthquake => Build<BeijingEarthquake>(),
				// FANStudioSource.YunnanEarthquake => Build<YunnanEarthquake>(),
				// FANStudioSource.CWAEarthquake => Build<CWAEarthquake>(),
				FANStudioSource.CWAEEW => Build<CWAEEW>(),
				// FANStudioSource.JMAEEW => Build<JMAEEW>(),
				FANStudioSource.HKOEarthquake => Build<HKOEarthquake>(),
				FANStudioSource.USGSEarthquake => Build<USGSEarthquake>(),
				FANStudioSource.ShakeAlertEEW => Build<ShakeAlertEEW>(),
				FANStudioSource.EMSCEarthquake => Build<EMSCEarthquake>(),
				FANStudioSource.BCSFEarthquake => Build<BCSFEarthquake>(),
				FANStudioSource.GFZEarthquake => Build<GFZEarthquake>(),
				FANStudioSource.USPEarthquake => Build<USPEarthquake>(),
				FANStudioSource.KMAEarthquake => Build<KMAEarthquake>(),
				// FANStudioSource.KMAEEW => Build<KMAEEW>(),
				FANStudioSource.FSSNEarthquake => Build<FSSNEarthquake>(),
				// FANStudioSource.FSSNCMT => Build<FSSNCMT>(),
				_ => throw new NotSupportedException(),
			};
		}
		static FANStudioWorker<T> Build<T>() where T : class {
			if (!FANStudioSourceTypeInfoProvider.Instance.TryGetSource(typeof(T), out string? source))
				throw new NotSupportedException();
			if (!FANStudioSourceTypeInfoProvider.Instance.TryGetTypeInfo(source, out var typeInfo))
				throw new NotSupportedException();
			return new(new(new("wss://ws.fanstudio.tech/"), source), typeInfo);
		}
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CENCEarthquakeReportGeneratorBuilder : SimpleBuilder<CENCEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CENCEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CEAEEWReportGeneratorBuilder : SimpleBuilder<CEAEEWReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CEAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class NingxiaEarthquakeReportGeneratorBuilder : SimpleBuilder<NingxiaEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(NingxiaEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class GuangxiEarthquakeReportGeneratorBuilder : SimpleBuilder<GuangxiEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(GuangxiEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class ShanxiEarthquakeReportGeneratorBuilder : SimpleBuilder<ShanxiEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(ShanxiEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class BeijingEarthquakeReportGeneratorBuilder : SimpleBuilder<BeijingEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(BeijingEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class CWAEEWReportGeneratorBuilder : SimpleBuilder<CWAEEWReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(CWAEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class HKOEarthquakeReportGeneratorBuilder : SimpleBuilder<HKOEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(HKOEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class ShakeAlertEEWReportGeneratorBuilder : SimpleBuilder<ShakeAlertEEWReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(ShakeAlertEEW), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class EMSCEarthquakeReportGeneratorBuilder : SimpleBuilder<EMSCEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(EMSCEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class BCSFEarthquakeReportGeneratorBuilder : SimpleBuilder<BCSFEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(BCSFEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class GFZEarthquakeReportGeneratorBuilder : SimpleBuilder<GFZEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(GFZEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class USPEarthquakeReportGeneratorBuilder : SimpleBuilder<USPEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(USPEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class KMAEarthquakeReportGeneratorBuilder : SimpleBuilder<KMAEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(KMAEarthquake), ref culture);
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class FSSNEarthquakeReportGeneratorBuilder : SimpleBuilder<FSSNEarthquakeReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(nameof(FSSNEarthquake), ref culture);
	}
}
