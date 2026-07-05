using Cryville.Common.Compat;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.EMSC {
	public class EMSCRealTimeEventReportGenerator : IContextedGenerator<EMSCRealTimeAction, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(EMSCRealTimeAction e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			var ev = e.Event;
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = ev is null ? res.GetStringRequired("AuthorityName") : string.Format(culture, res.GetStringRequired("AuthorityNameForwarded"), ev.Authority),
			};

			if (e.Action == "delete") {
				// TODO Cancel existing if possible
				result.GroupKeys.Add(new ReportUnitKey(e.EventID, null));
				result.RevisionKey = new ReportRevisionKey(true);
				return result;
			}

			if (ev == null) return result;
			result.GroupKeys.Add(new ReportUnitKey(ev.UniqueID, ev.Authority));
			result.RevisionKey = new ReportRevisionKey();

			if (!context.NameLocationTo(result, ev.Latitude, ev.Longitude, Local.Culture, culture)) {
				result.Location = ev.FlynnRegion;
				result.LocationSpecificity = 3;
			}
			result.Time = ev.Time;
			result.TimeZone = TimeZoneInfo.Utc;

			result.GroupKeys.Add(new HypocenterGroupKey(ev.Latitude, ev.Longitude, ev.Time.UtcDateTime, ev.Magnitude, ev.Depth));
			result.Properties.Add(new(ev.MagnitudeType switch {
				"mb" => TagTypeKeys.MagnitudeBodyWave,
				"md" => TagTypeKeys.MagnitudeDuration,
				"ml" => TagTypeKeys.MagnitudeLocal,
				"mw" => TagTypeKeys.MagnitudeMoment,
				_ => TagTypeKeys.Magnitude,
			}, ev.MagnitudeType, ev.Magnitude.ToString("0.0#", CultureInfo.InvariantCulture), context.SeverityScheme, ev.Magnitude) { AccuracyOrder = 30 });
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), ev.Depth.ToString("0.##", culture)), context.SeverityScheme, ev.Depth) { AccuracyOrder = 30 });

			return result;
		}

		sealed record ReportUnitKey(string? ID, string? Authority) : IReportUnitKey;
		sealed record ReportRevisionKey(bool IsCancellation = false) : IReportRevisionKey;
	}
}
