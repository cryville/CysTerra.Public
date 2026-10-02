using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.NowQuakeCENCInt.Model;
using Cryville.EEW.Report;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cryville.EEW.NowQuakeCENCInt {
	public class NowQuakeCENCIntDataReportGenerator : IContextedGenerator<WsData, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(WsData e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			string? localLocationName = CENCHelpers.ExtractLocationAffixes(e.Hypocenter, out string? affixes, culture);
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Predicate = affixes,
				Time = new(e.HappenTime, Local.TimeZoneOffset),
				TimeZone = Local.TimeZone,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.EarthquakeId));
			result.RevisionKey = new ReportRevisionKey(e.UpdateTime);

			context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.HappenTime, result.TimeZone), e.Magnitude, e.Depth));

			if (e.MaxIntensity is float maxIntensity && (maxIntensity != 0 || (e.Stations is IReadOnlyCollection<Station> stations && stations.Count > 0)))
				result.Properties.Add(RomanNumerals.CreateRomanIntensityProperty(TagTypeKeys.IntensityCSIS, res.GetStringRequired("PropertyMaxIntensity"), maxIntensity, culture, context.SeverityScheme, 10));
			else
				result.Properties.Add(new(TagTypeKeys.IntensityCSIS, res.GetStringRequired("PropertyMaxIntensity"), "-", -1) { AccuracyOrder = 100 });

			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 10 });

			if (result.Location == null) {
				result.Location = localLocationName;
				result.LocationSpecificity = CENCHelpers.GetSpecificity(localLocationName);
			}

			if (e.Depth is float depth) result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth), context.SeverityScheme, depth) { AccuracyOrder = 10 });

			return result;
		}

		sealed record ReportUnitKey(string EarthquakeId) : IReportUnitKey;
		sealed record ReportRevisionKey(DateTime UpdateTime) : IReportRevisionKey {
			public bool IsComparableWith(IReportRevisionKey obj) => obj is ReportRevisionKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				return UpdateTime.CompareTo(other.UpdateTime);
			}
		}
	}
}
