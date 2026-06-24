using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public class CEAEEWReportGenerator : IContextedGenerator<CEAEEW, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(CEAEEW e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(CEAEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Location = e.PlaceName,
				LocationSpecificity = CENCHelpers.GetSpecificity(e.PlaceName),
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				InvalidatedTime = new DateTimeOffset(e.UpdateTime ?? e.ShockTime, Local.TimeZoneOffset) + TimeSpan.FromMinutes(5),
				TimeZone = Local.TimeZone,
			};

			result.GroupKeys.Add(new ReportUnitKey(e.EventID));
			context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude, e.Depth));
			result.RevisionKey = new ReportRevisionKey(e.Updates);
			if (e.EpicenterIntensity is float epiIntensity) result.Properties.Add(RomanNumerals.CreateRomanIntensityProperty("Intensity:CSIS", res.GetStringRequired("PropertyMaxIntensity"), epiIntensity, culture, context.SeverityScheme, 70));
			result.Properties.Add(new("Magnitude:SurfaceWave", res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 70 });
			if (e.Depth is float depth) result.Properties.Add(new("HypocenterDepth", res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth.ToString("F0", culture)), context.SeverityScheme, e.Depth) { AccuracyOrder = 70 });
			return result;
		}

		sealed record ReportUnitKey(string EventID) : IReportUnitKey { }
		sealed record ReportRevisionKey(int Updates) : IReportRevisionKey {
			public int? Serial => Updates;
		}
	}
}
