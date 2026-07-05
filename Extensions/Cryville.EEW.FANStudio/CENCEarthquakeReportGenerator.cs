using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public sealed class CENCEarthquakeReportGenerator : IContextedGenerator<CENCEarthquake, IReportGeneratorContext, ReportModel> {
		readonly static TagTypeKey TagQualityCENC = "Quality:CENC";

		public ReportModel Generate(CENCEarthquake? e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(CENCEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			string localLocationName = CENCHelpers.ExtractLocationAffixes(e.PlaceName, out string? affixes, culture);
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Predicate = affixes,
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				TimeZone = Local.TimeZone,
				Model = e,
			};
			string id = CENCHelpers.ExtractEventID(e.EventID, out _);
			result.GroupKeys.Add(new ReportUnitKey(id));

			bool reviewedFlag = e.AutoFlag == "I" || e.InfoTypeName.Contains("正式", StringComparison.Ordinal);
			result.RevisionKey = new ReportRevisionKey(reviewedFlag);
			int accuracy = reviewedFlag ? 10 : 30;

			context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude, e.Depth));
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = accuracy });

			if (result.Location == null) {
				result.Location = localLocationName;
				result.LocationSpecificity = CENCHelpers.GetSpecificity(localLocationName);
			}

			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth), context.SeverityScheme, e.Depth) { AccuracyOrder = accuracy });

			result.Properties.Add(new(TagQualityCENC, null, res.GetStringRequired(reviewedFlag ? "PropertyQualityValueReviewed" : "PropertyQualityValueAutomatic"), -1) { AccuracyOrder = accuracy });
			return result;
		}

		sealed record ReportUnitKey(string EventID) : IReportUnitKey { }
		sealed record ReportRevisionKey(bool IsReviewed) : IReportRevisionKey {
			public bool IsComparableWith(IReportRevisionKey obj) => obj is ReportRevisionKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				return IsReviewed.CompareTo(other.IsReviewed);
			}
		}
	}
}
