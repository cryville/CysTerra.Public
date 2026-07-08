using Cryville.Common.Compat;
using Cryville.EEW.JMA;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cryville.EEW.Wolfx.TTS {
	public class JMAEEWTTSMessageGenerator : IContextedGenerator<JMAEEW, ITTSMessageGeneratorContext, TTSEntry?> {
		readonly ReportUnitStateList _states = new();
		public TTSEntry? Generate(JMAEEW e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(JMAEEW), ref culture);
			var res = lres.RootMessageStringSet;

			if (e.EventId is not string id) return null;
			else if (e.Status is JMAEEWStatus.GeneralCancellation or JMAEEWStatus.DrillingCancellation) {
				_states.Invalidate(id);
				return new(culture, null, res.GetStringRequired("Cancel"), -110, "eew_update_cancel");
			}
			else if (e.HypocenterCode == null) return null;
			else return GenerateCore(e, context, culture, res, id);
		}

		TTSEntry? GenerateCore(JMAEEW e, ITTSMessageGeneratorContext context, CultureInfo culture, IMessageStringSet res, string id) {
			Debug.Assert(e.HypocenterCode != null);

			var soundLevel = int.TryParse(e.MaxIntensity?[0].ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var i0) ? i0 : 0;
			var pr = _states.Push(id, [soundLevel, e.ForecastAreas.Count]);
			if (e.SerialType == JMAEEWSerialType.Final) _states.Invalidate(id);

			if (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(e.OriginTime, Local.JapanTimeZone) >= context.NowcastWarningDelayTolerance) return null;

			var sb = new StringBuilder();
			if (e.Status is JMAEEWStatus.Drilling or JMAEEWStatus.DrillingCancellation) sb.Append(res.GetStringRequired("Drilling"));
			if (e.ForecastType == JMAEEWForecastType.Warning) {
				sb.Append(res.GetStringRequired("Title"));
				using var lresAreaForecastLocalE = JMAMessages.AreaForecastEEW(culture);
				var resAreaForecastLocalE = lresAreaForecastLocalE.RootMessageStringSet;
				List<string> warningAreas = [];
				var areas = e.ForecastAreas.Where(area => area.ForecastType == JMAEEWForecastType.Warning).Select(area => area.Code).ToList();
				GroupAreas(areas);
				foreach (var area in areas) {
					warningAreas.Add(resAreaForecastLocalE.GetString(area) ?? area);
				}
				sb.Append(string.Format(culture,
					res.GetStringRequired("Area"),
					string.Join(res.GetStringRequired("AreaDelimiter"), warningAreas)
				));
			}
			using var lresAreaEpicenter = JMAMessages.AreaEpicenter(culture);
			var resAreaEpicenter = lresAreaEpicenter.RootMessageStringSet;
			sb.Append(string.Format(culture,
				(e.HypocenterAccuracy != JMAEEWHypocenterAccuracy.PSWaveLevelExcessionOrIPF1PointOrAssumedHypocenterElement || e.MaxIntensity == null) ? res.GetStringRequired("Body") : res.GetStringRequired("BodyLowQuality"),
				e.SerialType == JMAEEWSerialType.Final ? res.GetStringRequired("SerialFinal") : string.Format(culture, res.GetStringRequired("Serial"), e.Serial),
				resAreaEpicenter.GetString(e.HypocenterCode), JMAMessageUtils.ToLongDisplayShindo(e.MaxIntensity, culture)
			));
			return new(
				culture, null, sb.ToString(),
				e.ForecastType == JMAEEWForecastType.Warning ? -105 : -100,
				pr.HasNewMaxState ? soundLevel switch {
					< 3 => "eew_1",
					3 => "eew_2",
					4 => "eew_3",
					5 => "eew_4",
					>= 6 => "eew_5",
				} : (e.SerialType == JMAEEWSerialType.Final ? "eew_update_final" : "eew_update")
			);
		}

		static readonly float[] _groupingRatios = [0, 0.5f];
		static void GroupAreas(List<string> areas) {
			foreach (float ratio in _groupingRatios) {
				bool flag = false;
				for (int i = 0; i < areas.Count; i++) {
					if (!JMAAreaForecastHierarchy.Instance.HierarchyParent.TryGetValue(areas[i], out var parent)) continue;
					var children = JMAAreaForecastHierarchy.Instance.HierarchyChildren[parent];
					int count = 0;
					foreach (var child in children) {
						if (areas.Contains(child)) count++;
					}
					if ((float)count / children.Length < ratio) continue;
					foreach (var child in children) {
						areas.Remove(child);
					}
					areas.Insert(i, parent);
					flag = true;
				}
				if (!flag) break;
			}
		}
	}
}
