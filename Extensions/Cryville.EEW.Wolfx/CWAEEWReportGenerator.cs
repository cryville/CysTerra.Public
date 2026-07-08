using Cryville.Common.Compat;
using Cryville.EEW.Report;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Globalization;

namespace Cryville.EEW.Wolfx {
	public sealed class CWAEEWReportGenerator : IContextedGenerator<CWAEEW, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(CWAEEW e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(CWAEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Source = res.GetStringRequired("AuthorityName"),
				TimeZone = Local.TaiwanTimeZone,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.ID));
			if (e.IsCancellation) {
				result.Title = res.GetStringRequired("TitleCancellation");
				result.RevisionKey = new ReportRevisionKey(0, true);
				return result;
			}
			result.Title = res.GetStringRequired("Title");
			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, Local.TaiwanCulture, culture)) {
				result.Location = e.HypoCenter;
				result.LocationSpecificity = 8;
			}
			result.Time = new(e.OriginTime, Local.TaiwanTimeZoneOffset);
			result.InvalidatedTime = new DateTimeOffset(e.ReportTime, Local.TaiwanTimeZoneOffset) + TimeSpan.FromMinutes(5);
			result.RevisionKey = new ReportRevisionKey(e.ReportNum);
			result.Properties.Add(new(TagTypeKeys.IntensityCWASIS, res.GetStringRequired("PropertyMaxIntensity"), e.MaxIntensity, context.SeverityScheme, e.MaxIntensity) { AccuracyOrder = 70 });
			result.Properties.Add(new(TagTypeKeys.MagnitudeRichter, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 70 });
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.OriginTime, result.TimeZone), e.Magnitude, e.Depth));
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth), context.SeverityScheme, e.Depth) { AccuracyOrder = 70 });
			return result;
		}

		sealed record ReportUnitKey(int ID) : IReportUnitKey { }
		sealed record ReportRevisionKey(int ReportNum, bool IsCancellation = false) : IReportRevisionKey {
			public int? Serial => ReportNum;
		}
	}
}
