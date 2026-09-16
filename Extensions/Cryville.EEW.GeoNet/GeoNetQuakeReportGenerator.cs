using Cryville.Common.Compat;
using Cryville.EEW.GeoJSON;
using Cryville.EEW.GeoNet.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.GeoNet {
	public class GeoNetQuakeReportGenerator : IContextedGenerator<Feature<GeoNetQuake>, IReportGeneratorContext, ReportModel> {
		static readonly TagTypeKey TagQuality = "Quality";

		public ReportModel Generate(Feature<GeoNetQuake> e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			return Generate(e.Geometry as Point, e.Properties, context, ref culture);
		}

		internal static ReportModel Generate(Point? point, GeoNetQuake e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("TitleQuake"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = e.Time,
				TimeZone = Local.TimeZone,
			};

			if (point == null || !context.NameLocationTo(result, point.Coordinates.Latitude, point.Coordinates.Longitude, Local.Culture, culture)) {
				result.Location = e.Locality;
				result.LocationSpecificity = 6;
			}

			result.GroupKeys.Add(new ReportUnitKey(e.PublicID));
			result.GroupKeys.Add(new GeoNetPublicIDGroupKey(e.PublicID));
			if (point != null) {
				result.GroupKeys.Add(new HypocenterGroupKey(point.Coordinates.Latitude, point.Coordinates.Longitude, e.Time.UtcDateTime, e.Magnitude, e.Depth));
			}
			if (e is GeoNetQuakeHistory history) {
				var reportTime = history.ModificationTime;
				result.UtcIssueTime = reportTime.UtcDateTime;
				result.RevisionKey = new ReportRevisionKey(reportTime, e.Quality == "deleted");
			}
			else {
				result.RevisionKey = new ReportRevisionKey(DateTimeOffset.UtcNow, e.Quality == "deleted");
			}

			var accuracyOrder = e.Quality switch {
				"best" => 10,
				"preliminary" => 15,
				"automatic" => 30,
				_ => 100,
			};
			if (e.MMI > 0)
				result.Properties.Add(new(TagTypeKeys.IntensityMMI, res.GetStringRequired("PropertyMaxIntensity"), RomanNumerals.ToRomanNumeralChar(e.MMI, culture), context.SeverityScheme, e.MMI) { AccuracyOrder = 50 });
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = accuracyOrder });
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth.ToString("F1", culture)), context.SeverityScheme, e.Depth) { AccuracyOrder = accuracyOrder });

			var qualityProp = new ReportProperty(TagQuality, null, res.GetStringSetRequired("PropertyQualityValue").GetStringOrDefault(e.Quality), -1) { AccuracyOrder = accuracyOrder };
			if (e.Quality == "deleted")
				result.Properties.Insert(0, qualityProp);
			else
				result.Properties.Add(qualityProp);

			return result;
		}

		sealed record ReportUnitKey(string PublicID) : IReportUnitKey;
		sealed record ReportRevisionKey(DateTimeOffset ReportTime, bool IsDeleted) : IReportRevisionKey {
			public bool IsCancellation => IsDeleted;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				return ReportTime.CompareTo(other.ReportTime);
			}
		}
	}
	public class GeoNetQuakeHistoryReportGenerator : IContextedGenerator<Feature<GeoNetQuakeHistory>, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(Feature<GeoNetQuakeHistory> e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			return GeoNetQuakeReportGenerator.Generate(e.Geometry as Point, e.Properties, context, ref culture);
		}
	}
}
