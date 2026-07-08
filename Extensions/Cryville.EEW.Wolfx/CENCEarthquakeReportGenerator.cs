using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.ComponentModel;
using Cryville.EEW.Report;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

namespace Cryville.EEW.Wolfx {
	public sealed class CENCEarthquakeReportGenerator : IContextedGenerator<WolfxEarthquakeList<CENCEarthquake>, IReportGeneratorContext, ReportModel>, IPropertiesHolder {
		readonly static TagTypeKey TagQualityCENC = "Quality:CENC";

		[LocalizableDisplayName("PNUseRawLocationName")]
		[LocalizableDescription("PDUseRawLocationName")]
		public bool UseRawLocationName { get; set; }

		readonly HashSet<CENCEarthquake> _history = [];
		public ReportModel Generate(WolfxEarthquakeList<CENCEarthquake> e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			ReportModel? result = null;
			lock (_history) {
				foreach (var eq in e.Earthquakes) {
					if (eq == null) continue;
					if (_history.Add(eq)) {
						result ??= Generate(eq, context, ref culture);
					}
				}
				_history.RemoveWhere(eq => !e.Earthquakes.Contains(eq));
			}
			return result ?? throw new JsonException("Got new report without new or updated event.");
		}
		ReportModel Generate(CENCEarthquake? e, IReportGeneratorContext context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			using var lres = new LocalizedResource(nameof(CENCEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			string localLocationName = CENCHelpers.ExtractLocationAffixes(UseRawLocationName ? (e.RawLocation ?? e.Location) : e.Location, out string? affixes, culture);
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Predicate = affixes,
				Time = new(e.Time, Local.TimeZoneOffset),
				TimeZone = Local.TimeZone,
				Model = e,
			};
			if (WolfxHelpers.ExtractEventID(e.EventID) is string id)
				result.GroupKeys.Add(new ReportUnitKey(id));

			bool reviewedFlag = e.Type == "reviewed";
			result.RevisionKey = new ReportRevisionKey(reviewedFlag);
			int accuracy = reviewedFlag ? 10 : 30;

			float? magnitude = float.TryParse(e.Magnitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var mag) ? mag : null;
			float? hypocenterDepth = float.TryParse(e.Depth, NumberStyles.Float, CultureInfo.InvariantCulture, out var depth) ? depth : null;
			if (
				float.TryParse(e.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
				float.TryParse(e.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon)
			) {
				context.NameLocationTo(result, lat, lon, Local.Culture, culture);
				if (magnitude != null) {
					result.GroupKeys.Add(new HypocenterGroupKey(lat, lon, TimeZoneInfo.ConvertTimeToUtc(e.Time, result.TimeZone), mag, hypocenterDepth));
				}
			}
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), magnitude is float mag2 ? mag2.ToString("F1", culture) : e.Magnitude, context.SeverityScheme, magnitude) { AccuracyOrder = accuracy });

			if (result.Location == null) {
				result.Location = localLocationName;
				result.LocationSpecificity = CENCHelpers.GetSpecificity(localLocationName);
			}

			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), hypocenterDepth is float depth2 ? depth2 : e.Depth), context.SeverityScheme, hypocenterDepth) { AccuracyOrder = accuracy });

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
