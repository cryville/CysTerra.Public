using Cryville.Common.Compat;
using Cryville.EEW.JMA;
using Cryville.EEW.JMAAtom.Model;
using Cryville.EEW.JMAAtom.Model.Seismology;
using Cryville.EEW.JMAAtom.Model.Volcanology;
using Cryville.EEW.Report;
using System;
using System.Globalization;
using System.Linq;

namespace Cryville.EEW.JMAAtom {
	public sealed class JMAAtomReportGenerator : IContextedGenerator<JMAReport, IReportGeneratorContext, ReportModel> {
		readonly static TagTypeKey TagTsunamiWarningJMA = "TsunamiWarning:JMA";
		readonly static TagTypeKey TagVolcanicWarningJMA = "VolcanicWarning:JMA";

		public ReportModel Generate(JMAReport e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			bool localFlag = culture.Equals(Local.Culture);
			int? serial = int.TryParse(e.Head.Serial, NumberStyles.Integer, CultureInfo.InvariantCulture, out var s) ? s : null;
			var result = new ReportModel {
				Title = localFlag ? e.Head.Title : res.GetStringSetRequired("InfoKind").GetStringOrDefault(e.Head.InfoKind),
				Source = res.GetStringRequired("AuthorityName"),
				Time = e.Head.TargetDateTime.Value,
				TimeZone = Local.TimeZone,
				RevisionKey = new ReportRevisionKey(e.Control.DateTime, serial, e.Head.InfoType == "取消"),
			};
			foreach (var id in e.Head.EventID.Split(' ')) {
				result.GroupKeys.Add(new JMAEventIdGroupKey(id));
				if (!string.IsNullOrEmpty(id)) {
					result.GroupKeys.Add(new JMAEventUnitKey(e.Control.Title, e.Control.EditorialOffice, e.Control.Status, id));
				}
			}
			if (result.UnitKeys.Count == 0) {
				result.GroupKeys.Add(new JMAEventUnitKey(e.Control.Title, e.Control.EditorialOffice, e.Control.Status, null));
			}
			if (JMAAtomHelpers.IsOverseaVolcanicEruption(e, out var seisBody2)) {
				GenerateFromOverseaVolcanicEruption(res, localFlag, culture, result, seisBody2);
				return result;
			}
			if (!localFlag && e.Head.InfoKind is "津波警報・注意報・予報") {
				if (e.Body is Model.Seismology.Body body && body.Tsunami?.Forecast is TsunamiDetail forecast) {
					var ress = res.GetStringSetRequired("TsunamiForecastCategory");
					string localizedTitle = string.Join(res.GetStringRequired("ShortItemSeparator"), forecast.Items
						.Select(i => i.Category.Kind.Code)
						.Distinct()
						.OrderByDescending(JMAMessageUtils.KeyTsunamiWarning)
						.Select(ress.GetString)
						.Where(n => n != null)
						.Distinct()
					);
					if (!string.IsNullOrWhiteSpace(localizedTitle))
						result.Title = localizedTitle;
				}
			}
			if (e.Head.ValidDateTime != default) {
				result.InvalidatedTime = e.Head.ValidDateTime.Value;
			}
			else if (e.Head.InfoKind is "震度速報") {
				result.InvalidatedTime = e.Head.ReportDateTime.Value + TimeSpan.FromMinutes(5);
			}
			else if (e.Head.InfoKind is "津波警報・注意報・予報") {
				// Tsunami Forecasts have ValidDateTime and do not go into this branch
				result.InvalidatedTime = DateTimeOffset.MaxValue;
			}
			if (e.Body is Model.Seismology.Body seisBody) {
				GenerateFromSeismologyBody(e, context, res, localFlag, culture, result, seisBody);
			}
			else if (e.Body is Model.Volcanology.Body volcBody) {
				GenerateFromVolcanologyBody(e, context, res, localFlag, culture, result, volcBody);
			}
			if (e.Control.Status != "通常") {
				result.Title = string.Format(culture, res.GetStringSetRequired("Status").GetString(e.Control.Status) ?? "{0}", result.Title);
			}
			return result;
		}

		static void GenerateFromOverseaVolcanicEruption(IMessageStringSet res0, bool localFlag, CultureInfo culture, ReportModel result, Model.Seismology.Body seisBody) {
			var res = res0.GetStringSetRequired("OverseaVolcanicEruption");
			result.Title = res.GetStringRequired("Title");
			if (seisBody.Earthquakes != null && seisBody.Earthquakes.Count > 0) {
				var eq = seisBody.Earthquakes.OrderByDescending(i => i.Magnitudes.Last().Value).First();
				GenerateFromHypocenter(res0, localFlag, culture, result, seisBody, eq);
				var coords = eq.Hypocenter.Area.Coordinates[^1];
				if (!string.IsNullOrEmpty(coords.Value)) {
					result.GroupKeys.Add(new HypocenterGroupKey(coords.Latitude, coords.Longitude, eq.OriginTime.Value.UtcDateTime, 0));
				}
			}
		}

		static void GenerateFromSeismologyBody(JMAReport e, IReportGeneratorContext context, IMessageStringSet res, bool localFlag, CultureInfo culture, ReportModel result, Model.Seismology.Body seisBody) {
			if (seisBody.Tsunami is Tsunami tsunami) {
				GenerateFromTsunami(e, context, res, culture, result, tsunami);
			}
			if (seisBody.Intensity is Intensity intensity) {
				GenerateFromIntensity(context, res, culture, result, intensity, e.Head.InfoKind is "震度速報" ? 30 : 10);
			}
			if (seisBody.Earthquakes != null && seisBody.Earthquakes.Count > 0) {
				var eq = seisBody.Earthquakes.OrderByDescending(i => i.Magnitudes.Last().Value).First();
				GenerateFromEarthquake(context, res, localFlag, culture, result, seisBody, eq, e.Head.InfoKind is "震源要素更新のお知らせ" ? 5 : 10);
			}
		}

		static void GenerateFromTsunami(JMAReport e, IReportGeneratorContext context, IMessageStringSet res, CultureInfo culture, ReportModel result, Tsunami tsunami) {
			if (e.Head.Title is "津波観測に関する情報" or "沖合の津波観測に関する情報") {
				GenerateFromTsunamiObservation(context, res, culture, result, tsunami);
			}
			else {
				GenerateFromTsunamiForecast(res, result, tsunami);
			}
		}

		static void GenerateFromTsunamiObservation(IReportGeneratorContext context, IMessageStringSet res, CultureInfo culture, ReportModel result, Tsunami tsunami) {
			float maxHeightValue = -2f;
			string? conditionStr = null;
			foreach (var station in tsunami.Observation.Items.SelectMany(i => i.Stations)) {
				var maxHeight = station.MaxHeight;
				if (maxHeight == null) continue;
				if (maxHeight.TsunamiHeight is MeasuredValue<float> tsunamiHeight) {
					if (tsunamiHeight.Value > maxHeightValue || (tsunamiHeight.Value == maxHeightValue && conditionStr == null)) {
						maxHeightValue = tsunamiHeight.Value;
						conditionStr = tsunamiHeight.Description switch {
							[.., '超'] => res.GetStringRequired("PropertyMaxTsunamiHeightConditionOver"),
							[.., '未', '満'] => res.GetStringRequired("PropertyMaxTsunamiHeightConditionBelow"),
							_ => null,
						};
					}
				}
				else if (maxHeight.Condition is string condition) {
					var conditions = condition.Split('\x3000');
					if (conditions.Contains("微弱") && maxHeightValue < 0) maxHeightValue = 0;
					if (conditions.Contains("観測中") && maxHeightValue < -0.5f) maxHeightValue = -0.5f;
					if (conditions.Contains("欠測") && maxHeightValue < -1) maxHeightValue = -1;
				}
			}
			if (maxHeightValue > 0) {
				result.Properties.Add(new(
					TagTypeKeys.TsunamiHeight,
					res.GetStringRequired("PropertyMaxTsunamiHeight"),
					string.Format(culture, res.GetStringRequired("PropertyMaxTsunamiHeightValue"), maxHeightValue.ToString("F1", culture)),
					context.SeverityScheme,
					maxHeightValue
				) { Condition = conditionStr });
			}
			else {
				result.Properties.Add(new(
					TagTypeKeys.TsunamiHeight,
					res.GetStringRequired("PropertyMaxTsunamiHeight"),
					res.GetStringSetRequired("PropertyMaxTsunamiHeightSpecialValue").GetString(maxHeightValue switch {
						-1 => "欠測",
						< 0 => "観測中",
						0 => "微弱",
						_ => "",
					}) ?? "",
					context.SeverityScheme,
					maxHeightValue >= 0 ? maxHeightValue : null
				) { Condition = conditionStr });
			}
		}

		static void GenerateFromTsunamiForecast(IMessageStringSet res, ReportModel result, Tsunami tsunami) {
			int level = tsunami.Forecast.Items.Max(i => i.Category.Kind.Code switch {
				"52" or "53" => 5,
				"51" => 4,
				"62" => 3,
				"71" or "72" or "73" => 0,
				_ => -4,
			});
			result.Properties.Add(new(TagTsunamiWarningJMA, null, res.GetStringSetRequired("PropertyTsunamiWarningValue").GetStringOrDefault(level.ToString(CultureInfo.InvariantCulture)), level / 4f) { AccuracyOrder = 70 });
			if (level < 0) result.InvalidatedTime = null;
		}

		static void GenerateFromIntensity(IReportGeneratorContext context, IMessageStringSet res, CultureInfo culture, ReportModel result, Intensity intensity, int accuracy) {
			if (intensity.Observation is IntensityDetail observation) {
				if (observation.MaxLgInt != null) {
					result.Properties.Add(new(TagTypeKeys.IntensityJMALPGM, res.GetStringRequired("PropertyMaxLPGM"), observation.MaxLgInt, context.SeverityScheme, observation.MaxLgInt) { AccuracyOrder = accuracy });
				}
				result.Properties.Add(new(TagTypeKeys.IntensityJMA, res.GetStringRequired("PropertyMaxIntensity"), JMAMessageUtils.ToLongDisplayShindo(observation.MaxInt, culture), context.SeverityScheme, observation.MaxInt) { AccuracyOrder = accuracy });
			}
		}

		static void GenerateFromEarthquake(IReportGeneratorContext context, IMessageStringSet res, bool localFlag, CultureInfo culture, ReportModel result, Model.Seismology.Body seisBody, Earthquake eq, int accuracy) {
			GenerateFromHypocenter(res, localFlag, culture, result, seisBody, eq);
			var coords = eq.Hypocenter.Area.Coordinates[^1];
			float magnitude = 0;
			foreach (var mag in eq.Magnitudes) {
				var type = mag.Type switch {
					"Mj" => "Magnitude:JMA",
					"M" => "Magnitude",
					_ => "Magnitude",
				};
				if (float.IsNaN(mag.Value)) {
					float sevMag = -1;
					if (mag.Description == "Ｍ８を超える巨大地震") {
						sevMag = context.SeverityScheme.From(type, 8f);
						magnitude = 8;
					}
					result.Properties.Add(new(type, null, res.GetStringSetRequired("PropertyMagnitudeValueUnknown").GetStringOrDefault(mag.Description), sevMag) { AccuracyOrder = accuracy });
				}
				else {
					result.Properties.Add(new(type, mag.Type, mag.Value.ToString("F1", culture), context.SeverityScheme, magnitude = mag.Value) { AccuracyOrder = accuracy });
				}
			}
			if (!string.IsNullOrEmpty(coords.Value)) {
				result.GroupKeys.Add(new HypocenterGroupKey(coords.Latitude, coords.Longitude, eq.OriginTime.Value.UtcDateTime, magnitude, coords.Height / -1000));
				if (coords.Height is double height) {
					if (height == 0) {
						result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), res.GetStringRequired("PropertyDepthValueVeryShallow"), context.SeverityScheme, 0) { AccuracyOrder = accuracy });
					}
					else {
						var h = -height / 1000;
						result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), h), context.SeverityScheme, h) { AccuracyOrder = accuracy });
					}
				}
			}
		}

		static void GenerateFromHypocenter(IMessageStringSet res, bool localFlag, CultureInfo culture, ReportModel result, Model.Seismology.Body seisBody, Earthquake eq) {
			var area = eq.Hypocenter.Area;
			if (localFlag) {
				result.Location = area.Name;
			}
			else {
				using var lresAreaEpicenter = JMAMessages.AreaEpicenter(culture);
				var resAreaEpicenter = lresAreaEpicenter.RootMessageStringSet;
				result.Location = resAreaEpicenter.GetString(area.Code.Value) ?? area.Name;
			}
			if (seisBody.Earthquakes.Count > 1)
				result.Location = string.Format(culture, res.GetStringRequired("LocationAggregated"), result.Location, seisBody.Earthquakes.Count - 1);
			result.LocationSpecificity = JMAMessageUtils.GetEpicenterAreaSpecificity(area.Code.Value);
			result.Time = eq.OriginTime.Value;
		}

		static void GenerateFromVolcanologyBody(JMAReport e, IReportGeneratorContext context, IMessageStringSet res, bool localFlag, CultureInfo culture, ReportModel result, Model.Volcanology.Body volcBody) {
			Item? item = null;
			Model.Volcanology.Area? volc = null;
			foreach (var i in volcBody.VolcanoInfo.SelectMany(vi => vi.Items).Where(i => i.Areas.CodeType == "火山名")) {
				item = i;
				foreach (var a in i.Areas.Items) {
					if (volc != null) {
						volc = null;
						goto exitIter;
					}
					volc = a;
				}
			}
		exitIter:
			if (volc != null && item != null) {
				if (localFlag) {
					result.Title = e.Control.Title;
					result.Location = volc.Name;
				}
				else {
					using var lresPointVolcano = JMAMessages.PointVolcano(culture);
					var resPointVolcano = lresPointVolcano.RootMessageStringSet;
					result.Location = resPointVolcano.GetString(volc.Code) ?? volc.Name;
				}
				result.LocationSpecificity = 12;
				if (int.TryParse(item.Kind.Code, NumberStyles.Integer, CultureInfo.InvariantCulture, out int code)) {
					if (code > 50) {
						result.Predicate = localFlag ? item.Kind.Name : res.GetStringSetRequired("PredicateVolcanic").GetStringOrDefault(item.Kind.Code, "52");
					}
					else if (code > 10) {
						result.Properties.Add(new(TagVolcanicWarningJMA, null, res.GetStringSetRequired("PropertyVolcanicWarningValue").GetStringOrDefault(item.Kind.Code), JMAMessageUtils.GetVolcanicWarningSeverity(item.Kind.Code)) { AccuracyOrder = 70 });
					}
				}
				if (item.EventTime is EventTime eventTime && eventTime.EventDateTime is JMADateTime time) {
					result.GroupKeys.Add(new JMAVolcanoEruptionGroupKey(volc.Code, time.Value.UtcDateTime));
				}
			}
			if (volcBody.VolcanoObservation is VolcanoObservation observation) {
				if (observation.ColorPlume is Plume colorPlume)
					GenerateFromPlume(result, context, res, localFlag, colorPlume, res.GetStringRequired("PlumeColorPlume"), culture, observation.WhitePlume == null);
				if (observation.WhitePlume is Plume whitePlume)
					GenerateFromPlume(result, context, res, localFlag, whitePlume, res.GetStringRequired("PlumeWhitePlume"), culture);
			}
		}

		static void GenerateFromPlume(ReportModel result, IReportGeneratorContext context, IMessageStringSet res, bool localFlag, Plume plume, string type, CultureInfo culture, bool noWhitePlumeFlag = false) {
			if (plume.PlumeHeightAboveCrater is not MeasuredValue<int> height) {
				return;
			}
			if (height.Condition == "噴煙なし") {
				if (noWhitePlumeFlag) result.Properties.Add(new(TagTypeKeys.PlumeHeightAboveCrater, null, res.GetStringRequired("PropertyPlumeHeightAboveCraterValueNone"), -1) { AccuracyOrder = 10 });
			}
			else if (height.Condition == "不明") {
				result.Properties.Add(new(
					TagTypeKeys.PlumeHeightAboveCrater,
					string.Format(culture, res.GetStringRequired("PropertyPlumeHeightAboveCrater"), type),
					res.GetStringRequired("PropertyPlumeHeightAboveCraterValueUnknown"),
					-1
				));
			}
			else {
				result.Properties.Add(new(
					TagTypeKeys.PlumeHeightAboveCrater,
					string.Format(culture, res.GetStringRequired("PropertyPlumeHeightAboveCrater"), type),
					string.Format(culture, res.GetStringRequired("PropertyPlumeHeightAboveCraterValue"), height.Value / 1000f),
					context.SeverityScheme,
					height.Value
				) { Condition = height.Condition != null ? (localFlag ? height.Condition : res.GetStringSetRequired("PropertyPlumeHeightAboveCraterValueCondition").GetString(height.Condition)) : null });
			}
		}

		sealed record ReportRevisionKey(DateTimeOffset DateTime, int? Serial, bool IsCancellation = false) : IReportRevisionKey {
			public bool IsComparableWith(IReportRevisionKey obj) => obj is ReportRevisionKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				return DateTime.CompareTo(other.DateTime);
			}
		}
	}
}
