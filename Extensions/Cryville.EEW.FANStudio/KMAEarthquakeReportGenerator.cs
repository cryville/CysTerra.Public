using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public sealed class KMAEarthquakeReportGenerator : IContextedGenerator<KMAEarthquake, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(KMAEarthquake? e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(KMAEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.ShockTime, Local.SouthKoreaTimeZoneOffset),
				TimeZone = Local.SouthKoreaTimeZone,
				Model = e,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.ID));
			result.RevisionKey = new ReportRevisionKey();

			context.NameLocationTo(result, e.Latitude, e.Longitude, Local.SouthKoreaCulture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, Local.TimeZone), e.Magnitude));
			int? maxIntensity = e.EpicenterIntensity;
			if (maxIntensity is int maxIntensityValue)
				result.Properties.Add(new(TagTypeKeys.IntensityMMI, res.GetStringRequired("PropertyMaxIntensity"), RomanNumerals.ToRomanNumeralChar(maxIntensityValue, culture), context.SeverityScheme, maxIntensity) { AccuracyOrder = 10 });
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 10 });
			if (e.Depth is float depth) result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth), context.SeverityScheme, depth) { AccuracyOrder = 10 });

			if (result.Location == null) {
				result.Location = e.PlaceName;
				result.LocationSpecificity = 6;
			}

			return result;
		}

		sealed record ReportUnitKey(string ID) : IReportUnitKey { }
		sealed record ReportRevisionKey : IReportRevisionKey;
	}
}
