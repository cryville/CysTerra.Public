using Cryville.Common.Compat;
using Cryville.EEW.JMA;
using Cryville.EEW.Report;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Globalization;

namespace Cryville.EEW.Wolfx {
	public sealed class JMAEEWReportGenerator : IContextedGenerator<JMAEEW, IReportGeneratorContext, ReportModel> {
		readonly static TagTypeKey TagQualityJMAEEW = "Quality:JMAEEW";

		public ReportModel Generate(JMAEEW e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(JMAEEW), ref culture);
			var res = lres.RootMessageStringSet;
			if (e.EventId is null) throw new ArgumentException("Invalid event ID.");
			var result = new ReportModel {
				Source = res.GetStringRequired("AuthorityName"),
				TimeZone = Local.JapanTimeZone,
			};
			result.GroupKeys.Add(new JMAEventUnitKey(
				"緊急地震速報（予報）",
				e.PublishingOffice switch {
					JMAEEWPublishingOffice.Sapporo => "札幌管区気象台",
					JMAEEWPublishingOffice.Sendai => "仙台管区気象台",
					JMAEEWPublishingOffice.Tokyo => "気象庁本庁",
					JMAEEWPublishingOffice.Osaka => "大阪管区気象台",
					JMAEEWPublishingOffice.Fukuoka => "福岡管区気象台",
					_ => "気象庁本庁",
				},
				e.Status switch {
					JMAEEWStatus.General or JMAEEWStatus.GeneralCancellation => "通常",
					JMAEEWStatus.Drilling or JMAEEWStatus.DrillingCancellation => "訓練",
					JMAEEWStatus.ReferenceOrTest or JMAEEWStatus.AllCodeTest => "試験",
					_ => "通常",
				},
				e.EventId
			));
			result.GroupKeys.Add(new JMAEventIdGroupKey(e.EventId));
			if (e.Status is JMAEEWStatus.GeneralCancellation or JMAEEWStatus.DrillingCancellation) {
				result.Title = res.GetStringRequired("TitleCancellation");
				result.RevisionKey = new ReportRevisionKey(e.Serial, IsCancellation: true);
				return result;
			}
			else {
				using var lresAreaEpicenter = JMAMessages.AreaEpicenter(culture);
				var resAreaEpicenter = lresAreaEpicenter.RootMessageStringSet;
				result.Title = res.GetStringRequired(e.ForecastType switch {
					JMAEEWForecastType.Forecast => "TitleForecast",
					JMAEEWForecastType.Warning => "TitleWarning",
					_ => "Title",
				});
				result.Location = resAreaEpicenter.GetString(e.HypocenterCode ?? "") ?? e.HypocenterCode;
				result.LocationSpecificity = JMAMessageUtils.GetEpicenterAreaSpecificity(e.HypocenterCode);
				result.Time = new(e.OriginTime, Local.JapanTimeZoneOffset);
				result.InvalidatedTime = new DateTimeOffset(e.DateTime, Local.JapanTimeZoneOffset) + TimeSpan.FromMinutes(5);
				result.RevisionKey = new ReportRevisionKey(e.Serial, e.SerialType == JMAEEWSerialType.Final);
				result.Properties.Add(new(TagTypeKeys.IntensityJMA, res.GetStringRequired("PropertyMaxIntensity"), JMAMessageUtils.ToLongDisplayShindo(e.MaxIntensity, culture), context.SeverityScheme, e.MaxIntensity is string maxIntensity ? maxIntensity : null) { AccuracyOrder = 70 });
				float magnitude = 0, sevMag = -1;
				if (e.Magnitude is float mag) {
					sevMag = context.SeverityScheme.From(TagTypeKeys.MagnitudeJMA, magnitude = mag);
				}
				bool hasLowHypocenterAccuracy = e.HypocenterAccuracy == JMAEEWHypocenterAccuracy.PSWaveLevelExcessionOrIPF1PointOrAssumedHypocenterElement;
				if (hasLowHypocenterAccuracy && e.Magnitude == null) {
					result.Properties.Add(new(TagQualityJMAEEW, null, res.GetStringRequired("PropertyQualityValueLevel"), -1) { AccuracyOrder = 70 });
				}
				else if (hasLowHypocenterAccuracy && e.Magnitude == 1.0f) {
					result.Properties.Add(new(TagQualityJMAEEW, null, res.GetStringRequired("PropertyQualityValuePLUM"), -1) { AccuracyOrder = 70 });
				}
				else {
					result.Properties.Add(new(TagTypeKeys.MagnitudeJMA, res.GetStringRequired("PropertyMagnitude"), e.Magnitude?.ToString("F1", culture) ?? res.GetStringRequired("PropertyMagnitudeValueUnknown"), sevMag) { AccuracyOrder = 70 });
					if (e.Latitude is float lat && e.Longitude is float lon) {
						result.GroupKeys.Add(new HypocenterGroupKey(lat, lon, TimeZoneInfo.ConvertTimeToUtc(e.OriginTime, result.TimeZone), magnitude, e.Depth));
					}
					if (e.Depth is int depth) {
						result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), depth == 0 ? res.GetStringRequired("PropertyDepthValueVeryShallow") : string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth), context.SeverityScheme, depth) { AccuracyOrder = 70 });
					}
					else {
						result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), res.GetStringRequired("PropertyDepthValueUnknown"), -1) { AccuracyOrder = 70 });
					}
				}
			}
			if (e.Status is JMAEEWStatus.Drilling or JMAEEWStatus.DrillingCancellation) {
				result.Title = string.Format(culture, res.GetStringRequired("StatusDrilling"), result.Title);
			}
			else if (e.Status is JMAEEWStatus.ReferenceOrTest or JMAEEWStatus.AllCodeTest) {
				result.Title = string.Format(culture, res.GetStringRequired("StatusTesting"), result.Title);
			}
			return result;
		}

		sealed record ReportRevisionKey(int? Serial, bool IsFinalRevision = false, bool IsCancellation = false) : IReportRevisionKey { }
	}
}
