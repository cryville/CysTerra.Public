using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public class ICLEEWReportGenerator : IContextedGenerator<ICLEEW, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(ICLEEW e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(ICLEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				InvalidatedTime = new DateTimeOffset(e.UpdateTime, Local.TimeZoneOffset) + TimeSpan.FromMinutes(5),
				TimeZone = Local.TimeZone,
			};

			result.GroupKeys.Add(new ReportUnitKey(e.EventID));
			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture)) {
				result.Location = e.PlaceName;
				result.LocationSpecificity = CENCHelpers.GetSpecificity(e.PlaceName);
			}
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude, e.Depth));
			result.RevisionKey = new ReportRevisionKey(e.Updates);
			result.Properties.Add(RomanNumerals.CreateRomanIntensityProperty(TagTypeKeys.IntensityCSIS, res.GetStringRequired("PropertyMaxIntensity"), e.EpicenterIntensity, culture, context.SeverityScheme, 70));
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 70 });
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth.ToString("F0", culture)), context.SeverityScheme, e.Depth) { AccuracyOrder = 70 });
			return result;
		}

		sealed record ReportUnitKey(string EventID) : IReportUnitKey { }
		sealed record ReportRevisionKey(int Updates) : IReportRevisionKey {
			public int? Serial => Updates;
		}
	}
}
