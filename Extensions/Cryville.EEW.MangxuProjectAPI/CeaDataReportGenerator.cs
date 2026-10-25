using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.MangxuProjectAPI.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.MangxuProjectAPI {
	public class CeaDataReportGenerator : IContextedGenerator<CeaData, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(CeaData e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(CeaData), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				InvalidatedTime = new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) + TimeSpan.FromMinutes(5),
				TimeZone = Local.TimeZone,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.EventId));
			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture)) {
				result.Location = e.PlaceName;
				result.LocationSpecificity = CENCHelpers.GetSpecificity(e.PlaceName);
			}
			result.RevisionKey = new ReportRevisionKey(e.Updates);
			if (e.EpiIntensity is float epiIntensity)
				result.Properties.Add(RomanNumerals.CreateRomanIntensityProperty(TagTypeKeys.IntensityCSIS, res.GetStringRequired("PropertyMaxIntensity"), epiIntensity, culture, context.SeverityScheme, 70));
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 70 });
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude, e.Depth));
			if (e.Depth is float depth) {
				result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth), context.SeverityScheme, depth) { AccuracyOrder = 70 });
			}
			return result;
		}

		sealed record ReportUnitKey(string Id) : IReportUnitKey { }
		sealed record ReportRevisionKey(int Updates) : IReportRevisionKey {
			public int? Serial => Updates;
		}
	}
}
