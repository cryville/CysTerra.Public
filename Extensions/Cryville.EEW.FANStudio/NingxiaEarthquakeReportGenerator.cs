using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public sealed class NingxiaEarthquakeReportGenerator : IContextedGenerator<NingxiaEarthquake, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(NingxiaEarthquake? e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(NingxiaEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				TimeZone = Local.TimeZone,
				Model = e,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.ID));
			result.RevisionKey = new ReportRevisionKey();

			context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude, e.Depth));
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 10 });

			if (result.Location == null) {
				result.Location = e.PlaceName;
				result.LocationSpecificity = 6;
			}

			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth), context.SeverityScheme, e.Depth) { AccuracyOrder = 10 });

			return result;
		}

		sealed record ReportUnitKey(string EventID) : IReportUnitKey { }
		sealed record ReportRevisionKey : IReportRevisionKey;
	}
}
