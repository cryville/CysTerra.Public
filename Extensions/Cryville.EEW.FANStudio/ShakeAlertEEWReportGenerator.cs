using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public class ShakeAlertEEWReportGenerator : IContextedGenerator<ShakeAlertEEW, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(ShakeAlertEEW e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(ShakeAlertEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var utcTime = TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, Local.TimeZone);
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Location = e.PlaceName,
				LocationSpecificity = 8,
				Time = new(utcTime, TimeSpan.Zero),
				InvalidatedTime = new DateTimeOffset(utcTime, TimeSpan.Zero) + TimeSpan.FromMinutes(5),
				TimeZone = TimeZoneInfo.Utc,
			};

			result.GroupKeys.Add(new ReportUnitKey(e.ID));
			result.RevisionKey = new ReportRevisionKey();
			context.NameLocationTo(result, e.Latitude, e.Longitude, Local.UnitedStatesCulture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, utcTime, e.Magnitude ?? 0, e.Depth));

			if (e.Magnitude is float mag) result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), mag.ToString("F1", culture), context.SeverityScheme, mag) { AccuracyOrder = 70 });
			if (e.Depth is float depth) result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth.ToString("F0", culture)), context.SeverityScheme, depth) { AccuracyOrder = 70 });
			return result;
		}

		sealed record ReportUnitKey(string ID) : IReportUnitKey { }
		sealed record ReportRevisionKey : IReportRevisionKey;
	}
}
