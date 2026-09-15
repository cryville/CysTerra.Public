using Cryville.Common.Compat;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.BMKGOpenData {
	public partial class BMKGEarthquakeReportGenerator : IContextedGenerator<BMKGEarthquake, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(BMKGEarthquake e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			bool localFlag = culture.Equals(Local.Culture);
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = e.DateTime,
				TimeZone = Local.TimeZone,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.DateTime, e.Coordinates, e.Magnitude, e.Depth, e.Region));
			result.RevisionKey = new ReportRevisionKey();

			if (e.Intensity is string intensity && BMKGHelpers.GetMaxIntensity(intensity) is string maxIntensity) {
				if (string.IsNullOrEmpty(maxIntensity))
					result.Properties.Add(new(TagTypeKeys.IntensityMMI, res.GetStringRequired("PropertyMaxIntensity"), "?", -1) { AccuracyOrder = 90 });
				else
					result.Properties.Add(new(TagTypeKeys.IntensityMMI, res.GetStringRequired("PropertyMaxIntensity"), maxIntensity, context.SeverityScheme, RomanNumerals.RomanToInteger(maxIntensity)) { AccuracyOrder = 50 });
			}

			float? magnitude = float.TryParse(e.Magnitude, NumberStyles.Float, CultureInfo.InvariantCulture, out float mag) ? mag : null;
			float? hypocenterDepth = BMKGHelpers.ExtractDepth(e.Depth);
			if (BMKGHelpers.ExtractCoordinates(e.Coordinates, out float lat, out float lon)) {
				var locCulture = culture;
				context.NameLocationTo(result, lat, lon, Local.Culture, culture);
				if (magnitude != null) {
					result.GroupKeys.Add(new HypocenterGroupKey(lat, lon, e.DateTime.UtcDateTime, magnitude.Value, hypocenterDepth));
				}
			}

			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), magnitude is float mag2 ? mag2.ToString("F1", culture) : e.Magnitude, context.SeverityScheme, magnitude) { AccuracyOrder = 10 });

			if (result.Location == null) {
				result.Location = BMKGHelpers.NormalizeLocation(e.Region);
				result.LocationSpecificity = 5;
			}

			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), localFlag ? e.Depth : string.Format(culture, res.GetStringRequired("PropertyDepthValue"), hypocenterDepth), context.SeverityScheme, hypocenterDepth) { AccuracyOrder = 10 });

			return result;
		}

		sealed record ReportUnitKey(DateTimeOffset DateTime, string Coordinates, string Magnitude, string Depth, string Region) : IReportUnitKey;
		sealed record ReportRevisionKey : IReportRevisionKey;
	}
}
