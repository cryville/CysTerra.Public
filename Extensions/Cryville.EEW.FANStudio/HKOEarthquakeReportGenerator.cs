using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public sealed class HKOEarthquakeReportGenerator : IContextedGenerator<HKOEarthquake, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(HKOEarthquake? e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(HKOEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				TimeZone = Local.TimeZone,
				Model = e,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.EventID));

			bool verifiedFlag = e.VerificationStatus == "Y";
			result.RevisionKey = new ReportRevisionKey(verifiedFlag);
			int accuracy = verifiedFlag ? 10 : 30;

			context.NameLocationTo(result, e.Latitude, e.Longitude, Local.HongKongCulture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude, e.Depth));
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = accuracy });

			if (result.Location == null) {
				result.Location = e.Region;
				result.LocationSpecificity = 3;
			}

			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth), context.SeverityScheme, e.Depth) { AccuracyOrder = accuracy });

			return result;
		}

		sealed record ReportUnitKey(string EventID) : IReportUnitKey { }
		sealed record ReportRevisionKey(bool IsVerified) : IReportRevisionKey {
			public bool IsComparableWith(IReportRevisionKey obj) => obj is ReportRevisionKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				return IsVerified.CompareTo(other.IsVerified);
			}
		}
	}
}
