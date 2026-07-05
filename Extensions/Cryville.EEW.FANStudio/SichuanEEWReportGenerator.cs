using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public class SichuanEEWReportGenerator : IContextedGenerator<SichuanEEW, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(SichuanEEW e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(SichuanEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				InvalidatedTime = new DateTimeOffset(e.CreateTime, Local.TimeZoneOffset) + TimeSpan.FromMinutes(5),
				TimeZone = Local.TimeZone,
			};

			string id = CENCHelpers.ExtractEventID(e.EventID, out _);
			result.GroupKeys.Add(new ReportUnitKey(id));
			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture)) {
				result.Location = e.PlaceName;
				result.LocationSpecificity = 6;
			}
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude));
			result.RevisionKey = new ReportRevisionKey(e.Updates, e.CreateTime);
			if (e.EpicenterIntensity is float epiIntensity) result.Properties.Add(RomanNumerals.CreateRomanIntensityProperty(TagTypeKeys.IntensityCSIS, res.GetStringRequired("PropertyMaxIntensity"), epiIntensity, culture, context.SeverityScheme, 70));
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 70 });
			return result;
		}

		sealed record ReportUnitKey(string EventID) : IReportUnitKey { }
		sealed record ReportRevisionKey(int? Revision, DateTime UpdateTime) : IReportRevisionKey {
			public int? Serial => Revision;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				if (Revision is not int rev) {
					if (other.Revision != null) return -1;
				}
				else if (other.Revision is not int rev2) {
					return 1;
				}
				else {
					int c = rev.CompareTo(rev2);
					if (c != 0) return c;
				}
				return UpdateTime.CompareTo(other.UpdateTime);
			}
		}
	}
}
