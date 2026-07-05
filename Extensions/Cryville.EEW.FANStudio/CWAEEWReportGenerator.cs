using Cryville.Common.Compat;
using Cryville.EEW.Report;
using Cryville.EEW.FANStudio.Model;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
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
			result.Title = res.GetStringRequired("Title");
			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, Local.TaiwanCulture, culture)) {
				result.Location = e.PlaceName;
				result.LocationSpecificity = 8;
			}
			result.Time = new(e.ShockTime, Local.TimeZoneOffset);
			result.InvalidatedTime = new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) + TimeSpan.FromMinutes(5);
			result.RevisionKey = new ReportRevisionKey(e.Updates);
			result.Properties.Add(new(TagTypeKeys.MagnitudeRichter, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 70 });
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude, e.Depth));
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth), context.SeverityScheme, e.Depth) { AccuracyOrder = 70 });
			return result;
		}
		public static string ExtractIntensity(int epicenterIntensity) => epicenterIntensity switch {
			5 => "5弱",
			6 => "5強",
			7 => "6弱",
			8 => "6強",
			9 => "7",
			_ => epicenterIntensity.ToString(CultureInfo.InvariantCulture),
		};

		sealed record ReportUnitKey(string ID) : IReportUnitKey { }
		sealed record ReportRevisionKey(int Updates) : IReportRevisionKey {
			public int? Serial => Updates;
		}
	}
}
