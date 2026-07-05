using Cryville.Common.Compat;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.GlobalQuake {
	public sealed class GlobalQuakeReportGenerator : IContextedGenerator<GlobalQuakeReport, IReportGeneratorContext, ReportModel> {
		static readonly int[] _timeoutTable = [
			3, 3, // M0
			3, 3, // M1
			3, 3, // M2
			5, 6, // M3
			8, 16, // M4
			30, 30, // M5
			30, 40, // M6
			40, 40, // M7+
		];

		readonly static TagTypeKey TagQualityGlobalQuake = "Quality:GlobalQuake";

		public ReportModel Generate(GlobalQuakeReport e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Source = res.GetStringRequired("AuthorityName"),
			};
			result.GroupKeys.Add(new ReportUnitKey(e.Id));
			result.RevisionKey = new ReportRevisionKey(e.RevisionId);
			if (e.RevisionId == -1) {
				result.Title = res.GetStringRequired("TitleCanceled");
				return result;
			}
			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, null, culture)) {
				result.Location = e.Region;
				result.LocationSpecificity = 3;
			}
			result.Time = new(e.OriginTime, TimeSpan.Zero);
			result.TimeZone = TimeZoneInfo.Utc;
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, e.OriginTime, e.Magnitude, e.Depth));
			result.UtcIssueTime = e.LastUpdatedTime;
			if (e.IsArchive) {
				result.Title = res.GetStringRequired("TitleArchived");
			}
			else {
				result.Title = res.GetStringRequired("Title");
				result.InvalidatedTime = new(e.OriginTime + TimeSpan.FromMinutes(1.5 * _timeoutTable[
					(int)Math.Clamp((e.Magnitude + Math.Log10(e.Depth + 160) - Math.Log10(160)) * 2, 0, _timeoutTable.Length - 1)
				]), TimeSpan.Zero);
			}
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 90 });
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth.ToString("F1", culture)), context.SeverityScheme, e.Depth) { AccuracyOrder = 90 });
			if (e.Quality is IHypocenterQualityData quality) {
				string qualityLevel = quality.QualityLevel switch {
					0 => "S",
					1 => "A",
					2 => "B",
					3 => "C",
					4 => "D",
					5 => "E",
					6 => "F",
					_ => "?",
				};
				result.Properties.Add(new(TagQualityGlobalQuake, res.GetStringRequired("PropertyQuality"), res.GetStringSetRequired("PropertyQualityValue").GetStringOrDefault(qualityLevel), context.SeverityScheme, qualityLevel) { AccuracyOrder = 90 });
			}
			return result;
		}

		sealed record ReportUnitKey(Guid EventId) : IReportUnitKey { }
		sealed record ReportRevisionKey(int RevisionId) : IReportRevisionKey {
			public int? Serial => RevisionId > 0 ? RevisionId : null;
			public bool IsCancellation => RevisionId == -1;
			public bool IsFinalRevision => RevisionId == 0;

			public bool IsComparableWith(IReportRevisionKey obj) => obj is ReportRevisionKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				if (RevisionId == -1) return other.RevisionId == -1 ? 0 : 1;
				if (other.RevisionId == -1) return -1;
				if (RevisionId == 0) return other.RevisionId == 0 ? 0 : 1;
				if (other.RevisionId == 0) return -1;
				return RevisionId.CompareTo(other.RevisionId);
			}
		}
	}
}
