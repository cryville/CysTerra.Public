using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.Report;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Globalization;

namespace Cryville.EEW.Wolfx {
	public sealed class CENCEEWReportGenerator : IContextedGenerator<CENCEEW, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(CENCEEW e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(CENCEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.OriginTime, Local.TimeZoneOffset),
				InvalidatedTime = new DateTimeOffset(e.ReportTime, Local.TimeZoneOffset) + TimeSpan.FromMinutes(5),
				TimeZone = Local.TimeZone,
			};
			if (WolfxHelpers.ExtractEventID(e.EventID) is string id)
				result.GroupKeys.Add(new ReportUnitKey(id));
			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture)) {
				result.Location = e.HypoCenter;
				result.LocationSpecificity = CENCHelpers.GetSpecificity(e.HypoCenter);
			}
			result.RevisionKey = new ReportRevisionKey(e.ReportNum);
			result.Properties.Add(RomanNumerals.CreateRomanIntensityProperty(TagTypeKeys.IntensityCSIS, res.GetStringRequired("PropertyMaxIntensity"), e.MaxIntensity, culture, context.SeverityScheme, 70));
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 70 });
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.OriginTime, result.TimeZone), e.Magnitude, e.Depth));
			if (e.Depth is float depth) {
				result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth), context.SeverityScheme, depth) { AccuracyOrder = 70 });
			}
			return result;
		}

		sealed record ReportUnitKey(string EventID) : IReportUnitKey { }
		sealed record ReportRevisionKey(int ReportNum) : IReportRevisionKey {
			public int? Serial => ReportNum;
		}
	}
}
