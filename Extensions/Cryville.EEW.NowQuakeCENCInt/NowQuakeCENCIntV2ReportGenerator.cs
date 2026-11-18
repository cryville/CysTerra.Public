using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.NowQuakeCENCInt.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.NowQuakeCENCInt {
	public class NowQuakeCENCIntV2ReportGenerator : IContextedGenerator<V2Report, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(V2Report e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;

			var metadata = e.Metadata;
			var data = e.Data;
			var ev = data.Event;
			var epicenter = ev.Epicenter;
			string? localLocationName = CENCHelpers.ExtractLocationAffixes(epicenter.Name, out string? affixes, culture);
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Predicate = affixes,
				Time = ev.OriginTime,
				TimeZone = Local.TimeZone,
				UtcIssueTime = ev.ReportTime?.UtcDateTime ?? DateTime.UtcNow,
			};
			var reportIdentity = metadata.Identity;
			var reportState = metadata.State;
			result.GroupKeys.Add(new ReportUnitKey(reportIdentity.EventId));
			result.RevisionKey = new ReportRevisionKey(reportIdentity.ReportNumber, reportState.Deleted, reportState.Blocked);

			if (epicenter.Latitude is double latitude && epicenter.Longitude is double longitude) {
				context.NameLocationTo(result, latitude, longitude, Local.Culture, culture);
				if (ev.OriginTime is DateTimeOffset originTime && ev.Magnitude is double magnitude) {
					result.GroupKeys.Add(new HypocenterGroupKey(latitude, longitude, originTime.UtcDateTime, magnitude, ev.Depth));
				}
			}

			if (data.Details.MaximumIntensity is double maxIntensity && (maxIntensity != 0 || (data.Stations is { } stations && stations.Count > 0)))
				result.Properties.Add(RomanNumerals.CreateRomanIntensityProperty(TagTypeKeys.IntensityCSIS, res.GetStringRequired("PropertyMaxIntensity"), (float)maxIntensity, culture, context.SeverityScheme, 10));
			else
				result.Properties.Add(new(TagTypeKeys.IntensityCSIS, res.GetStringRequired("PropertyMaxIntensity"), "-", -1) { AccuracyOrder = 100 });

			if (ev.Magnitude is double mag)
				result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), mag.ToString("F1", culture) ?? "?", context.SeverityScheme, mag) { AccuracyOrder = 10 });

			if (result.Location == null) {
				result.Location = localLocationName;
				result.LocationSpecificity = CENCHelpers.GetSpecificity(localLocationName);
			}

			if (ev.Depth is double depth)
				result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth), context.SeverityScheme, depth) { AccuracyOrder = 10 });

			return result;
		}

		sealed record ReportUnitKey(string EventId) : IReportUnitKey;
		sealed record ReportRevisionKey(int ReportNumber, bool IsDeleted, bool IsBlocked) : IReportRevisionKey {
			public int? Serial => ReportNumber;
			public bool IsCancellation => IsDeleted || IsBlocked;
			public bool IsComparableWith(IReportRevisionKey obj) => obj is ReportRevisionKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				return ReportNumber.CompareTo(other.ReportNumber);
			}
		}
	}
}
