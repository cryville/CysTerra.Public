using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.MangxuProjectAPI.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.MangxuProjectAPI {
	public class CeicRawReportGenerator : IContextedGenerator<CeicRaw, IReportGeneratorContext, ReportModel> {
		readonly static TagTypeKey TagQualityCENC = "Quality:CENC";

		public ReportModel Generate(CeicRaw e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(CeicRaw), ref culture);
			var res = lres.RootMessageStringSet;
			string localLocationName = CENCHelpers.ExtractLocationAffixes(e.LocName, out string? affixes, culture);
			var time = DateTimeOffset.FromUnixTimeMilliseconds(e.Time);
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Predicate = affixes,
				Time = time,
				TimeZone = Local.TimeZone,
				Model = e,
			};
			result.GroupKeys.Add(new ReportUnitKey(e.Eqid));

			bool reviewedFlag = e.DeterminationType == "正式测定";
			result.RevisionKey = new ReportRevisionKey(reviewedFlag);
			int accuracy = reviewedFlag ? 10 : 30;

			float depth = e.Depth / 1000f;
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, time.UtcDateTime, e.Mag, depth));

			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture)) {
				result.Location = localLocationName;
				result.LocationSpecificity = CENCHelpers.GetSpecificity(localLocationName);
			}

			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Mag.ToString("F1", culture), context.SeverityScheme, e.Mag) { AccuracyOrder = accuracy });
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth), context.SeverityScheme, depth) { AccuracyOrder = accuracy });

			result.Properties.Add(new(TagQualityCENC, null, res.GetStringRequired(reviewedFlag ? "PropertyQualityValueReviewed" : "PropertyQualityValueAutomatic"), -1) { AccuracyOrder = accuracy });
			return result;
		}

		sealed record ReportUnitKey(string Eqid) : IReportUnitKey { }
		sealed record ReportRevisionKey(bool IsReviewed) : IReportRevisionKey {
			public bool IsComparableWith(IReportRevisionKey obj) => obj is ReportRevisionKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				return IsReviewed.CompareTo(other.IsReviewed);
			}
		}
	}
}
