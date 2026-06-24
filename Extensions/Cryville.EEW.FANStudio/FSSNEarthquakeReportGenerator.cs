using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public sealed class FSSNEarthquakeReportGenerator : IContextedGenerator<FSSNEarthquake, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(FSSNEarthquake? e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(FSSNEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				TimeZone = Local.TimeZone,
				Model = e,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.ID));

			bool reviewedFlag = e.InfoTypeName.Contains("正式", StringComparison.Ordinal);
			result.RevisionKey = new ReportRevisionKey();
			int accuracy = reviewedFlag ? 10 : 15;

			context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude, e.Depth));
			result.Properties.Add(new("Magnitude", res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = accuracy });

			if (result.Location == null) {
				result.Location = e.PlaceNameZh;
				result.LocationSpecificity = 3;
			}

			result.Properties.Add(new("HypocenterDepth", res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth.ToString("F0", culture)), context.SeverityScheme, e.Depth) { AccuracyOrder = accuracy });

			return result;
		}

		sealed record ReportUnitKey(string EventID) : IReportUnitKey { }
		sealed record ReportRevisionKey : IReportRevisionKey;
	}
}
