using Cryville.Common.Compat;
using Cryville.EEW.JMA;
using Cryville.EEW.JMAAtom.Model;
using Cryville.EEW.JMAAtom.Model.Seismology;
using Cryville.EEW.JMAAtom.Model.Volcanology;
using Cryville.EEW.TTS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryville.EEW.JMAAtom.TTS {
	public partial class JMAAtomTTSMessageGenerator : IContextedGenerator<JMAReport, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(JMAReport e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			if (
				(e.Head.InfoKind is "地震の活動状況等に関する情報") &&
				(e.Head.Headline.Text is "南海トラフ地震に関連する情報（臨時）を発表します。" or "南海トラフ地震に関連する情報（定例）を発表します。")
			) {
				return null; // 移行措置用 XML 電文
			}
			var sb = new StringBuilder();
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			bool localFlag = culture.Equals(Local.Culture);
			if (e.Control.Status != "通常") sb.AppendLine(res.GetStringSet("Status")?.GetString(e.Control.Status));
			if (JMAAtomHelpers.IsOverseaVolcanicEruption(e, out var seisBody2)) {
				return GenerateOverseaVolcanicEruptionMessage(culture, sb, res, localFlag, seisBody2);
			}
			var title = localFlag ? e.Head.InfoKind : res.GetStringSet("Title")?.GetString(e.Head.InfoKind) ?? "";
			if (!GenerateSpecialHead(e, sb, res, localFlag, e.Head))
				sb.AppendLine(localFlag ? e.Head.ToString() : title);
			string? urgentSound = null;
			if (e.Head.InfoType == "取消" && !localFlag) {
				GenerateLocalizedCancellationMessage(e, culture, sb, res);
			}
			else if (e.Body is Model.Seismology.Body body) {
				GenerateSeismologyBodyMessage(e, culture, sb, res, localFlag, body, out urgentSound);
			}
			else if (e.Body is Model.Volcanology.Body vbody) {
				GenerateVolcanologyBodyMessage(e, culture, sb, res, localFlag, vbody);
			}
			return new(
				culture, title, sb.ToString(),
				e.Head.InfoType switch {
					"取消" => -10,
					_ => e.Head.InfoKind switch {
						"津波警報・注意報・予報" => -5,
						"地震情報" => -1,
						"降灰予報" => 10,
						_ => 0,
					}
				},
				urgentSound != null ? null : e.Head.InfoType switch {
					"取消" => "eq_c",
					_ => e.Head.InfoKind switch {
						"震度速報" => "eq_i",
						"震源速報" or "震源要素更新のお知らせ" or "地震回数情報" => "eq",
						"地震情報" or "長周期地震動に関する観測情報" => "eq_d",
						"津波情報" => "ev",
						"噴火警報・予報" or "噴火に関する火山観測報" => "vol_2",
						"降灰予報" or "火山の状況に関する解説情報" => "ev_vol",
						"南海トラフ地震に関連する情報" => e.Control.Title switch {
							"南海トラフ地震関連解説情報" => "ev_info",
							_ => "ev_breaking",
						},
						_ => "ev_breaking",
					}
				}
			) {
				UrgentEntry = urgentSound != null
					? new(Local.Culture, null, "", -1000, urgentSound) { IssueTime = e.Head.ReportDateTime.Value.ToUniversalTime().DateTime }
					: null
			};
		}

		static TTSEntry? GenerateOverseaVolcanicEruptionMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res0, bool localFlag, Model.Seismology.Body body) {
			var res = res0.GetStringSetRequired("OverseaVolcanicEruption");
			string title = res.GetStringRequired("Title");
			sb.AppendLine(title);
			if (body.Earthquakes.FirstOrDefault() is not Earthquake eq) throw new FormatException("Expect at least one earthquake in oversea volcanic eruption report.");
			sb.AppendLine(string.Format(culture, res.GetStringRequired("Headline"), TimeZoneInfo.ConvertTime(eq.OriginTime.Value, Local.TimeZone)));
			sb.AppendLine();

			if (body.Comments?.ForecastComment is CommentForm comment) {
				static string? ReplaceCommentKeywords(IMessageStringSet res, string? text) => text
					?.Replace(res.GetStringRequired("ReplaceEruptionSource"), res.GetStringRequired("ReplaceEruptionTarget"), StringComparison.Ordinal)
					?.Replace(res.GetStringRequired("ReplaceVolcanoSource"), res.GetStringRequired("ReplaceVolcanoTarget"), StringComparison.Ordinal);

				if (localFlag) {
					sb.AppendLine();
					sb.AppendLine(ReplaceCommentKeywords(res, comment.Text));
				}
				else {
					foreach (var code in comment.Code.Split(' ')) {
						sb.AppendLine();
						sb.AppendLine(ReplaceCommentKeywords(res, res0.GetStringSet("AdditionalCommentEarthquake")?.GetString(code)));
					}
				}
				sb.AppendLine();
			}

			var area = eq.Hypocenter.Area;
			using var lresAreaEpicenter = JMAMessages.AreaEpicenter(culture);
			var resAreaEpicenter = lresAreaEpicenter.RootMessageStringSet;
			sb.AppendLine(string.Format(culture,
				res.GetStringRequired("Area"),
				localFlag ? area.Name : resAreaEpicenter.GetString(area.Code.Value) ?? area.Name
			));

			return new(culture, title, sb.ToString(), -1, "jp/VXSE53");
		}

#if NET7_0_OR_GREATER
		[GeneratedRegex(@"^火　　山：(.*)\n日　　時：(.*)\p{Ps}.*\p{Pe}(.*?)\n現　　象：(.*)$", RegexOptions.Singleline)]
		private static partial Regex VolcanicEruptionHeadlineRegex();
#else
		static readonly Regex r_VolcanicEruptionHeadlineRegex = new(@"^火　　山：(.*)\n日　　時：(.*)\p{Ps}.*\p{Pe}(.*?)\n現　　象：(.*)$", RegexOptions.Singleline);
		static Regex VolcanicEruptionHeadlineRegex() => r_VolcanicEruptionHeadlineRegex;
#endif
		static bool GenerateSpecialHead(JMAReport e, StringBuilder sb, IMessageStringSet res, bool localFlag, Head head) {
			if (localFlag && head.InfoKind == "噴火に関する火山観測報") {
				sb.AppendLine(head.Title);
				sb.AppendLine();

				var headline = head.Headline;
				sb.AppendLine(VolcanicEruptionHeadlineRegex().Replace(headline.Text, "火山：$1\n日時：$2 $3\n現象：$4"));
				sb.AppendLine();

				var info = headline.Information;
				sb.AppendLine(info == null ? "" : string.Join("\n\n", info));

				return true;
			}
			if (!localFlag && head.InfoKind == "津波警報・注意報・予報") {
				if (e.Body is not Model.Seismology.Body body || body.Tsunami?.Forecast is not TsunamiDetail forecast)
					return false;
				var ress1 = res.GetStringSetRequired("TsunamiForecastCategoryShort");
				var ress2 = res.GetStringSetRequired("TsunamiForecastCategory");
				string localizedTitle = string.Join(res.GetStringRequired("ShortItemSeparator"), forecast.Items
					.Select(i => i.Category.Kind.Code)
					.Distinct()
					.OrderByDescending(JMAMessageUtils.KeyTsunamiWarning)
					.Select(k => ress1.GetString(k) ?? ress2.GetString(k))
					.Where(n => n != null)
					.Distinct()
				);
				if (string.IsNullOrWhiteSpace(localizedTitle))
					return false;
				sb.AppendLine(localizedTitle);
				return true;
			}

			return false;
		}

		static void GenerateLocalizedCancellationMessage(JMAReport e, CultureInfo culture, StringBuilder sb, IMessageStringSet res) {
			var ress = res.GetStringSet("Title");
			sb.AppendLine(string.Format(culture, res.GetStringRequired("Cancel"), ress?.GetStringOrDefault(e.Head.InfoKind)));
		}

		static void GenerateSeismologyBodyMessage(JMAReport e, CultureInfo culture, StringBuilder sb, IMessageStringSet res, bool localFlag, Model.Seismology.Body body, out string? urgentSound) {
			var bodyText = body.Text;
			if (!string.IsNullOrEmpty(bodyText) && localFlag) {
				sb.AppendLine(bodyText);
			}
			GenerateSpecialSeismologyHeadlineAndUrgentSound(e, culture, sb, res, localFlag, body, out urgentSound);
			if (body.Comments is Comment comments && comments.ForecastComment is CommentForm forecastComment) {
				GenerateCommentMessage(sb, res, localFlag, forecastComment);
			}
			if (body.Tsunami is Tsunami tsunami)
				GenerateTsunamiMessage(e, culture, sb, res, localFlag, tsunami);
			if (body.Intensity is Intensity intensity && e.Head.InfoKind != "津波情報")
				GenerateIntensityMessage(culture, sb, res, localFlag, intensity, e.Head.InfoKind == "震度速報" ? 4 : 10);
			if (body.Earthquakes != null && body.Earthquakes.Count > 0 && e.Head.InfoKind is not ("長周期地震動に関する観測情報" or "津波情報")) {
				bool multiFlag = body.Earthquakes.Count > 1;
				if (multiFlag) sb.AppendLine(res.GetStringRequired("EarthquakeMultiple"));
				int eqIndex = 1;
				foreach (var eq in body.Earthquakes) {
					if (multiFlag) sb.AppendLine(string.Format(culture, res.GetStringRequired("EarthquakeMultipleIndex"), eqIndex++));
					if (eq.Hypocenter is Hypocenter hypocenter) GenerateHypocenterMessage(culture, sb, res, localFlag, hypocenter);
					var magnitude = eq.Magnitudes[0];
					GenerateMagnitudeMessage(culture, sb, res, magnitude);
				}
			}
			if (body.EarthquakeCount is EarthquakeCount eqCounts) {
				GenerateEarthquakeCountMessage(culture, sb, res, eqCounts);
			}
			if (body.EarthquakeInfo is EarthquakeInfo eqInfo) {
				GenerateEarthquakeInfoMessage(sb, res, localFlag, eqInfo);
			}
			if (body.Comments is Comment comments2 && comments2.WarningComment is CommentForm warningComment) {
				GenerateCommentMessage(sb, res, localFlag, warningComment);
			}
			if (body.NextAdvisory is string nextAdvisory && localFlag) {
				sb.AppendLine(nextAdvisory);
			}
		}

		static void GenerateSpecialSeismologyHeadlineAndUrgentSound(JMAReport e, CultureInfo culture, StringBuilder sb, IMessageStringSet res, bool localFlag, Model.Seismology.Body body, out string? urgentSound) {
			urgentSound = null;
			if (e.Head.InfoKind == "津波警報・注意報・予報") {
				if (body.Tsunami is not Tsunami tsunami) return;
				if (tsunami.Forecast is not TsunamiDetail forecast) return;
				var categories = forecast.Items.Select(i => i.Category).ToArray();
				var kind = categories.OrderByDescending(i => JMAMessageUtils.KeyTsunamiWarning(i.Kind.Code)).First().Kind;
				var lastKind = categories.OrderByDescending(i => JMAMessageUtils.KeyTsunamiWarning(i.LastKind.Code)).First().LastKind;
				var kindKey = JMAMessageUtils.KeyTsunamiWarning(kind.Code);
				var lastKindKey = JMAMessageUtils.KeyTsunamiWarning(lastKind.Code);
				if (!localFlag) {
					if (kindKey > lastKindKey)
						sb.AppendLine(string.Format(
							culture, res.GetStringRequired("HeadlineTsunamiWarning"),
							res.GetStringSetRequired("TsunamiForecastCategory").GetString(kind.Code)
						));
					else if (kindKey == lastKindKey)
						sb.AppendLine(string.Format(
							culture, res.GetStringRequired("HeadlineTsunamiWarningUpdate"),
							res.GetStringSetRequired("TsunamiForecastCategory").GetString(kind.Code)
						));
					else if (kindKey > 0)
						sb.AppendLine(string.Format(
							culture, res.GetStringRequired("HeadlineTsunamiWarningDowngraded"),
							res.GetStringSetRequired("TsunamiForecastCategory").GetString(lastKind.Code),
							res.GetStringSetRequired("TsunamiForecastCategory").GetString(kind.Code)
						));
					else
						sb.AppendLine(string.Format(
							culture, res.GetStringRequired("HeadlineTsunamiWarningLifted"),
							res.GetStringSetRequired("TsunamiForecastCategory").GetString(lastKind.Code)
						));
				}
				if (kindKey > lastKindKey) {
					if (kindKey >= 2)
						urgentSound = "japanTsunamiWarningBegin";
				}
			}
			else if (localFlag)
				return;
			else if (e.Head.InfoKind == "津波情報")
				sb.AppendLine(res.GetStringRequired(e.Head.Title == "各地の満潮時刻・津波到達予想時刻に関する情報" ? "HeadlineTsunamiInformationForecast" : "HeadlineTsunamiInformationObservation"));
			else if (body.Earthquakes != null && body.Earthquakes.Count > 0)
				sb.AppendLine(string.Format(culture, res.GetStringRequired("HeadlineEarthquake"), body.Earthquakes[0].OriginTime));
			else if (body.Intensity != null)
				sb.AppendLine(string.Format(culture, res.GetStringRequired("HeadlineIntensity"), e.Head.TargetDateTime));
		}

		static void GenerateCommentMessage(StringBuilder sb, IMessageStringSet res, bool localFlag, CommentForm comment) {
			if (localFlag) {
				sb.AppendLine();
				sb.AppendLine(comment.Text);
			}
			else {
				foreach (var code in comment.Code.Split(' ')) {
					sb.AppendLine();
					sb.AppendLine(res.GetStringSet("AdditionalCommentEarthquake")?.GetString(code));
				}
			}
			sb.AppendLine();
		}

		static void GenerateTsunamiMessage(JMAReport e, CultureInfo culture, StringBuilder sb, IMessageStringSet res, bool localFlag, Tsunami tsunami) {
			using var lresAreaTsunami = JMAMessages.AreaTsunami(culture);
			var resAreaTsunami = lresAreaTsunami.RootMessageStringSet;
			IEnumerable<(TsunamiItem, TsunamiStation)> revisedObservation = [];
			IEnumerable<TsunamiItem> revisedForecast = [];
			if (tsunami.Observation is TsunamiDetail tsuObservation1) {
				revisedObservation = [.. tsuObservation1.Items
					.SelectMany(i => i.Stations, (area, station) => (area, station))
					.Where(i => i.station.MaxHeight?.Revise != null)];
			}
			if (tsunami.Forecast is TsunamiDetail tsuForecast1) {
				revisedForecast = [.. tsuForecast1.Items.Where(i => i.FirstHeight?.Revise != null || i.MaxHeight?.Revise != null)];
			}
			if (tsunami.Observation is TsunamiDetail tsuObservation) {
				var set = revisedObservation.Any() ? revisedObservation : (e.Head.Title is "津波観測に関する情報" or "沖合の津波観測に関する情報" ? tsuObservation.Items.SelectMany(i => i.Stations, (area, station) => (area, station)) : revisedObservation);
				using var lresPointTsunami = JMAMessages.PointTsunami(culture);
				var resPointTsunami = lresPointTsunami.RootMessageStringSet;
				foreach (var (area, station) in set) {
					GenerateTsunamiObservationMessage(culture, sb, res, localFlag, resAreaTsunami, resPointTsunami, area, station);
				}
			}
			if (tsunami.Forecast is TsunamiDetail tsuForecast) {
				var set = revisedForecast.Any() ? revisedForecast : (e.Head.InfoKind == "津波警報・注意報・予報" || e.Head.Title == "各地の満潮時刻・津波到達予想時刻に関する情報" ? tsuForecast.Items : revisedForecast);
				foreach (var category in set.GroupBy(i => JMAMessageUtils.KeyTsunamiWarning(i.Category.Kind.Code)).OrderByDescending(i => i.Key)) {
					GenerateTsunamiForecastMessage(culture, sb, res, localFlag, resAreaTsunami, category);
				}
				if (e.Head.InfoKind != "津波情報") {
					sb.AppendLine();
					sb.AppendLine(res.GetStringRequired("TsunamiForecastSuffix"));
				}
			}
		}

		static void GenerateTsunamiObservationMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res, bool localFlag, IMessageStringSet resAreaTsunami, IMessageStringSet resPointTsunami, TsunamiItem area, TsunamiStation station) {
			var h = station.MaxHeight;
			if (h == null) return;
			var th = h.TsunamiHeight;
			string thStr = "";
			if (th == null) {
				if (localFlag) {
					thStr = h.Condition;
				}
				else if (h.Condition != null) {
					bool flag = false;
					foreach (var c in h.Condition.Split('\x3000').Where(c => c != "重要")) {
						if (flag) thStr += res.GetStringRequired("MediumItemSeparator");
						else flag = true;
						thStr += res.GetStringSet("TsunamiObservationMaxHeightCondition")?.GetString(c) ?? c;
					}
				}
			}
			else {
				thStr = GenerateTsunamiHeightMessage(culture, res, th);
			}
			if (string.IsNullOrEmpty(area.Area.Name)) {
				sb.Append(string.Format(
					culture, res.GetStringRequired("TsunamiObservationStationNoArea"),
					localFlag ? station.Name : resPointTsunami.GetString(station.Code) ?? station.Name,
					thStr
				));
			}
			else {
				sb.Append(string.Format(
					culture, res.GetStringRequired("TsunamiObservationStation"),
					localFlag ? area.Area.Name : resAreaTsunami.GetString(area.Area.Code) ?? area.Area.Name,
					localFlag ? station.Name : resPointTsunami.GetString(station.Code) ?? station.Name,
					thStr
				));
			}
			if (th != null && th.Condition == "上昇中") sb.Append(res.GetStringRequired("TsunamiObservationMaxHeightRising"));
			sb.AppendLine();
		}

		static void GenerateTsunamiForecastMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res, bool localFlag, IMessageStringSet resAreaTsunami, IGrouping<int, TsunamiItem> category) {
			if (category.Key <= 0) return;
			var kind = category.First().Category.Kind;
			sb.AppendLine();
			sb.AppendLine(string.Format(culture, res.GetStringRequired("TsunamiForecast"), res.GetStringSet("TsunamiForecastCategory")?.GetString(kind.Code) ?? kind.Name));
			foreach (var area in category.Where(i => i.FirstHeight != null)) {
				sb.Append(string.Format(
					culture, res.GetStringRequired("TsunamiForecastArea"),
					localFlag ? area.Area.Name : resAreaTsunami.GetString(area.Area.Code) ?? area.Area.Name,
					area.FirstHeight.Condition != null
						? (localFlag ? area.FirstHeight.Condition : res.GetStringSet("TsunamiForecastFirstHeightCondition")?.GetString(area.FirstHeight.Condition) ?? area.FirstHeight.Condition)
						: string.Format(culture, res.GetStringRequired("TsunamiForecastFirstHeightArrivalTime"), area.FirstHeight.ArrivalTime)
				));
				if (area.MaxHeight is MaxHeight maxHeight) {
					var h = maxHeight.TsunamiHeight;
					if (!float.IsNaN(h.Value)) {
						sb.Append(string.Format(culture, res.GetStringRequired("TsunamiForecastMaxHeight"), GenerateTsunamiHeightMessage(culture, res, h)));
					}
				}
				sb.AppendLine();
			}
		}

		static string GenerateTsunamiHeightMessage(CultureInfo culture, IMessageStringSet res, MeasuredValue<float> height) => string.Format(
			culture, res.GetStringRequired(height.Description switch {
				[.., '超'] => "TsunamiHeightValueAffixOver",
				[.., '未', '満'] => "TsunamiHeightValueAffixBelow",
				_ => "TsunamiHeightValueAffix",
			}),
			string.Format(culture, res.GetStringRequired("TsunamiHeightValue"), height.Value)
		);

		static void GenerateIntensityMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res, bool localFlag, Intensity intensity, int areaCountThreshold = 10) {
			if (intensity.Observation is not IntensityDetail detail) return;
			if (!localFlag) {
				using var lresArea = JMAMessages.AreaForecastEEW(culture);
				var resArea = lresArea.RootMessageStringSet;
				string intName;
				int count = 0;
				var areas = detail.Prefs.SelectMany(pref => pref.Areas);
				IEnumerable<IGrouping<string, IntensityArea>> enumerable;
				if (detail.MaxLgInt != default) {
					intName = res.GetStringRequired("LongIntensity");
					enumerable = areas.GroupBy(area => area.MaxLgInt);
				}
				else {
					intName = res.GetStringRequired("Intensity");
					enumerable = areas.Where(area => area.MaxInt is not ("1" or "2")).GroupBy(area => area.MaxInt);
				}
				enumerable = enumerable.OrderByDescending(group => group.Key);
				bool anyGroup = false;
				foreach (var group in enumerable) {
					if (group.Key == null) continue;
					if (count >= areaCountThreshold) break;
					if (!anyGroup) {
						anyGroup = true;
						sb.Append(res.GetStringRequired("IntensityObservation"));
					}
					string intPrefix = intName;
					if (count == 0) intPrefix = string.Format(culture, res.GetStringRequired("Maximum"), intPrefix);
					string areaStr = "";
					bool flag = false;
					foreach (var area in group) {
						if (flag) areaStr += res.GetStringRequired("MediumItemSeparator");
						else flag = true;
						areaStr += resArea.GetString(area.Code) ?? area.Name;
						count++;
					}
					sb.AppendLine(string.Format(culture,
						res.GetStringRequired("IntensityObservationArea"),
						intPrefix, JMAMessageUtils.ToLongDisplayShindo(group.Key, culture), areaStr
					));
				}
			}
			if (detail.MaxInt != default)
				sb.AppendLine(string.Format(culture,
					res.GetStringRequired("IntensityObservationMax"),
					JMAMessageUtils.ToLongDisplayShindo(detail.MaxInt, culture)
				));
			if (detail.MaxLgInt != default)
				sb.AppendLine(string.Format(culture,
					res.GetStringRequired("LongIntensityObservationMax"),
					detail.MaxLgInt
				));
		}

		static void GenerateHypocenterMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res, bool localFlag, Hypocenter hypocenter) {
			var area = hypocenter.Area;
			using var lresAreaEpicenter = JMAMessages.AreaEpicenter(culture);
			var resAreaEpicenter = lresAreaEpicenter.RootMessageStringSet;
			sb.AppendLine(string.Format(culture,
				res.GetStringRequired("EarthquakeHypocenterArea"),
				localFlag ? area.Name : resAreaEpicenter.GetString(area.Code.Value) ?? area.Name
			));
			var coords = area.Coordinates[^1];
			if (string.IsNullOrEmpty(coords.Value) && !string.IsNullOrEmpty(coords.Description))
				sb.AppendLine(coords.Description);
			else if (coords.Height == null)
				sb.AppendLine(res.GetStringRequired("EarthquakeHypocenterDepthUnknown"));
			else if (coords.Height == 0)
				sb.AppendLine(res.GetStringRequired("EarthquakeHypocenterDepth0"));
			else
				sb.AppendLine(string.Format(culture,
				res.GetStringRequired("EarthquakeHypocenterDepth"),
				-coords.Height / 1000
			));
		}

		static void GenerateMagnitudeMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res, Magnitude magnitude) {
			if (float.IsNaN(magnitude.Value)) {
				var ress = res.GetStringSetRequired("EarthquakeMagnitudeUnknown");
				sb.AppendLine(ress.GetStringOrDefault(magnitude.Description));
			}
			else {
				sb.AppendLine(string.Format(culture,
					res.GetStringRequired("EarthquakeMagnitude"),
					magnitude.Value
				));
			}
		}

		static void GenerateEarthquakeCountMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res, EarthquakeCount eqCounts) {
			foreach (var eqCount in eqCounts.Items) {
				string str = "";
				if (eqCount.Number > 0) str += string.Format(culture, res.GetStringRequired("EarthquakeCountNumber"), eqCount.Number);
				if (eqCount.FeltNumber > 0) str += string.Format(culture, res.GetStringRequired("EarthquakeCountFeltNumber"), eqCount.FeltNumber);
				str = string.Format(culture, res.GetStringRequired("EarthquakeCount"), eqCount.StartTime, eqCount.EndTime, str);
				if (eqCount.Type == "累積地震回数") str = string.Format(culture, res.GetStringRequired("EarthquakeCountTotal"), str);
				sb.AppendLine(str);
			}
		}

		static void GenerateEarthquakeInfoMessage(StringBuilder sb, IMessageStringSet res, bool localFlag, EarthquakeInfo eqInfo) {
			if (localFlag) {
				sb.AppendLine(eqInfo.Text);
				return;
			}
			if (eqInfo.InfoSerial is InfoSerial infoSerial) {
				if (infoSerial.CodeType is "地震関連情報番号コード") {
					sb.AppendLine(res.GetStringSet("NankaiTroughInfo")?.GetString(infoSerial.Code));
				}
			}
		}

		static void GenerateVolcanologyBodyMessage(JMAReport e, CultureInfo culture, StringBuilder sb, IMessageStringSet res, bool localFlag, Model.Volcanology.Body body) {
			if (body.Text is string bodyText && localFlag) {
				var trimmingIndex = bodyText.IndexOf("（参考）", StringComparison.Ordinal);
				if (trimmingIndex >= 0) bodyText = bodyText[..trimmingIndex];
				sb.AppendLine(bodyText);
			}
			if (!localFlag) GenerateLocalizedVolcanologyHeadline(e, culture, sb, res, body);
			if (body.AshInfos is AshInfos ashInfos) {
				GenerateAshInfosMessage(culture, sb, res, localFlag, ashInfos);
			}
			if (body.VolcanoObservation is VolcanoObservation observation) {
				GenerateVolcanoObservationMessage(culture, sb, res, observation);
			}
		}

		static void GenerateLocalizedVolcanologyHeadline(JMAReport e, CultureInfo culture, StringBuilder sb, IMessageStringSet res, Model.Volcanology.Body body) {
			using var lresPointVolcano = JMAMessages.PointVolcano(culture);
			var resPointVolcano = lresPointVolcano.RootMessageStringSet;
			Item? item = null;
			Model.Volcanology.Area? volc = null;
			foreach (var i in body.VolcanoInfo.SelectMany(vi => vi.Items).Where(i => i.Areas.CodeType == "火山名")) {
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
			if (volc == null || item == null) return;

			var volcanoName = resPointVolcano.GetString(volc.Code) ?? volc.Name;
			switch (e.Head.InfoKind) {
				case "降灰予報":
					if (e.Control.Title == "降灰予報（定時）") sb.AppendLine(string.Format(culture, res.GetStringRequired("HeadlineAshScheduled"), volcanoName));
					else if (e.Control.Title == "降灰予報（速報）") sb.AppendLine(string.Format(culture, res.GetStringRequired("HeadlineAshPreliminary"), volcanoName));
					else if (e.Control.Title == "降灰予報（詳細）") sb.AppendLine(string.Format(culture, res.GetStringRequired("HeadlineAshDetailed"), volcanoName));
					break;
				case "噴火に関する火山観測報" or "噴火速報":
					sb.AppendLine(string.Format(culture, res.GetStringRequired("HeadlineEruption"), e.Head.TargetDateTime, volcanoName, res.GetStringSet("VolcanicWarning")?.GetString(item.Kind?.Code ?? "") ?? item.Kind?.Name));
					break;
				case "推定噴煙流向報":
					sb.AppendLine(string.Format(culture, res.GetStringRequired("HeadlinePlume"), volcanoName));
					break;
				case "火山の状況に関する解説情報" or "噴火警報・予報":
					var ress1 = res.GetStringSet("HeadlineVolcanoWarning");
					var ress2 = res.GetStringSet("VolcanicWarning");
					sb.AppendLine(string.Format(
						culture, ress1?.GetStringOrDefault(item.Kind.Condition) ?? "",
						volcanoName,
						ress2?.GetString(item.LastKind?.Code ?? "") ?? item.LastKind?.Name,
						ress2?.GetString(item.Kind?.Code ?? "") ?? item.Kind?.Name
					));
					break;
			}
		}

		static void GenerateAshInfosMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res, bool localFlag, AshInfos ashInfos) {
			string result = "";
			foreach (var ash in ashInfos.Items) {
				string ashResult = "";
				bool flag = false;
				foreach (var item in ash.Items) {
					var kind = item.Kind;
					var property = kind.Property;
					var plumeDirection = property.PlumeDirection.Value;
					if (!flag) flag = true;
					else ashResult += res.GetStringRequired("MediumItemSeparator");
					ashResult += string.Format(
						culture, res.GetStringRequired("AshInfoItem"),
						string.Format(culture, res.GetStringRequired("PlumeDirectionValue"), res.GetStringSet("PlumeDirection")?.GetString(plumeDirection) ?? plumeDirection),
						property.Distance.Value,
						localFlag ? kind.Name : res.GetStringSet("VolcanicWarning")?.GetString(kind.Code) ?? kind.Name
					);
				}
				result += string.Format(culture, res.GetStringRequired("AshInfo"), ash.StartTime, ash.EndTime, ashResult) + "\n";
			}
			sb.AppendLine(result);
		}

		static void GenerateVolcanoObservationMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res, VolcanoObservation observation) {
			if (observation.WindAboveCrater is WindAboveCrater windAboveCrater) {
				foreach (var wind in windAboveCrater.WindAboveCraterElements.Where(w => w.HeightProperty == "代表高度")) {
					var ress1 = res.GetStringSet("WindDegreeValue");
					var ress2 = res.GetStringSet("WindSpeedValue");
					sb.AppendLine(string.Format(
						culture, res.GetStringRequired("VolcanoObservationWindAboveCrater"),
						wind.WindHeightAboveSeaLevel.Value,
						string.Format(culture, ress1?.GetStringOrDefault(wind.WindDegree.Condition ?? "") ?? "", wind.WindDegree.Value),
						string.Format(culture, ress2?.GetStringOrDefault(wind.WindDegree.Condition ?? "") ?? "", wind.WindSpeed.Value)
					));
				}
				sb.AppendLine();
			}
			if (observation.ColorPlume is Plume colorPlume) {
				GeneratePlumeMessage(culture, sb, res, colorPlume, "VolcanoObservationColorPlume");
			}
			if (observation.WhitePlume is Plume whitePlume) {
				GeneratePlumeMessage(culture, sb, res, whitePlume, "VolcanoObservationWhitePlume");
			}
		}

		static void GeneratePlumeMessage(CultureInfo culture, StringBuilder sb, IMessageStringSet res, Plume plume, string plumeTypeKey) {
			var plumeType = res.GetString(plumeTypeKey);
			if (plume.PlumeHeightAboveCrater is MeasuredValue<int> height) {
				if (height.Condition == "噴煙なし") {
					sb.AppendLine(string.Format(culture, res.GetStringRequired("VolcanoObservationPlumeNone"), plumeType));
					return;
				}
				sb.AppendLine(string.Format(culture, res.GetStringRequired("VolcanoObservationPlume"), plumeType));
				if (height.Condition == "不明") {
					sb.AppendLine(res.GetStringRequired("VolcanoObservationPlumeHeightAboveCraterUnknown"));
				}
				else {
					var ress = res.GetStringSet("VolcanoObservationPlumeHeightAboveCraterValue");
					sb.AppendLine(string.Format(
						culture, res.GetStringRequired("VolcanoObservationPlumeHeightAboveCrater"),
						string.Format(culture, ress?.GetStringOrDefault(height.Condition ?? "") ?? "", height.Value)
					));
				}
			}
			if (plume.PlumeDirection is MeasuredValue<string> direction && direction.Condition != "噴煙なし") {
				if (direction.Value == "流向不明") {
					sb.AppendLine(res.GetStringRequired("VolcanoObservationPlumeDirectionUnknown"));
				}
				else {
					sb.AppendLine(string.Format(culture, res.GetStringRequired("VolcanoObservationPlumeDirection"), res.GetStringSet("PlumeDirection")?.GetString(direction.Value) ?? direction.Value));
				}
			}
		}
	}
}
