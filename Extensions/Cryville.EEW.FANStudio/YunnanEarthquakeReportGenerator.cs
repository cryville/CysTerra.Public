using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public sealed class YunnanEarthquakeReportGenerator : IContextedGenerator<YunnanEarthquake, IReportGeneratorContext, ReportModel?> {
		public ReportModel? Generate(YunnanEarthquake? e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			if (e.Latitude is not float latitude || e.Longitude is not float longitude)
				return null;

			using var lres = new LocalizedResource(nameof(YunnanEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				TimeZone = Local.TimeZone,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.Id));
			result.RevisionKey = new ReportRevisionKey();

			float? primaryMagnitude = null;
			if (e.Magnitude is float magnitude) {
				primaryMagnitude ??= magnitude;
				result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), magnitude.ToString("F1", culture), context.SeverityScheme, magnitude) { AccuracyOrder = 10 });
			}
			if (e.MagnitudeL is float magnitudeL) {
				primaryMagnitude ??= magnitudeL;
				result.Properties.Add(new(TagTypeKeys.MagnitudeRichter, res.GetStringRequired("PropertyMagnitudeL"), magnitudeL.ToString("F1", culture), context.SeverityScheme, magnitudeL) { AccuracyOrder = 10 });
			}
			if (primaryMagnitude != null)
				result.GroupKeys.Add(new HypocenterGroupKey(latitude, longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), primaryMagnitude.Value, e.Depth));

			if (e.PlaceName == null) {
				context.NameLocationTo(result, latitude, longitude, CultureInfo.InvariantCulture, culture);
			}
			else if (!context.NameLocationTo(result, latitude, longitude, Local.Culture, culture)) {
				result.Location = e.PlaceName;
				result.LocationSpecificity = 6;
			}

			if (e.Depth is float depth)
				result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth), context.SeverityScheme, depth) { AccuracyOrder = 10 });

			return result;
		}

		sealed record ReportUnitKey(string Id) : IReportUnitKey { }
		sealed record ReportRevisionKey : IReportRevisionKey;
	}
}
