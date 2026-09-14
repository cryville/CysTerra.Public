using Cryville.Common.Compat;
using Cryville.EEW.CWA;
using Cryville.EEW.CWAOpenData.Model;
using Cryville.EEW.TTS;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cryville.EEW.CWAOpenData.TTS {
	public class CWATsunamiTTSMessageGenerator : IContextedGenerator<Tsunami, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(Tsunami e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			var sb = new StringBuilder();
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			bool localFlag = culture.Equals(Local.Culture);
			var locCulture = culture;
			var epicenter = e.EarthquakeInfo.Epicenter;
			CWAOpenDataUtils.GetCoordinates(epicenter, out float lat, out float lon);
			bool matchFlag = !context.NameLocation(lat, lon, Local.Culture, ref locCulture, out string? locationName);
			var title = localFlag ? e.ReportType : res.GetStringSet("Title")?.GetString(e.ReportType) ?? "";
			sb.AppendLine(title);
			if (localFlag) sb.AppendLine(CWAMessageUtils.ToHalfwidthDigits(e.ReportContent));
			else {
				if (e.EarthquakeInfo is EarthquakeInfo info) {
					sb.AppendLine(string.Format(
						culture, res.GetStringRequired("TsunamiEarthquakeBody"),
						TimeZoneInfo.ConvertTime(info.OriginTime, Local.TimeZone).DateTime,
						matchFlag ? epicenter.Location : locationName,
						info.EarthquakeMagnitude.MagnitudeValue
					));
				}
				sb.AppendLine(res.GetStringRequired(e.ReportType switch {
					"海嘯消息" => e.ReportColor == "綠色" ? "TsunamiBodyMessageGreen" : "TsunamiBodyMessage",
					"海嘯警報解除" => "TsunamiBodyLifted",
					_ => "TsunamiBodyWarning",
				}));
			}
			TTSEntry? urgentEntry = null;
			if (e.TsunamiWave is TsunamiWave tsunamiWave) {
				if (tsunamiWave.TsuStations is TsuStation[] stations && stations.Length > 0) {
					if (matchFlag) {
						sb.AppendLine(res.GetStringRequired("TsunamiObservation"));
						foreach (var station in stations) {
							sb.AppendLine(string.Format(
								culture, res.GetStringRequired("TsunamiObservationStation"),
								station.StationName, CWAOpenDataUtils.ToWaveHeight(station.WaveHeight)
							));
						}
					}
					else {
						sb.AppendLine(string.Format(culture, res.GetStringRequired("TsunamiObservationMaxHeight"), stations.Max(station => CWAOpenDataUtils.ToWaveHeight(station.WaveHeight))));
					}
				}
				if (tsunamiWave.WarningAreas is WarningArea[] areas && areas.Length > 0) {
					urgentEntry = new(culture, null, "", -1000, "genericTsunamiWarning");
					foreach (var heightGroup in areas.GroupBy(area => area.WaveHeight).OrderByDescending(g => CWAOpenDataUtils.KeyTsunamiForecastWaveHeight(g.Key))) {
						sb.AppendLine(string.Format(
							culture, res.GetStringRequired("TsunamiForecast"),
							localFlag ? heightGroup.Key : res.GetStringSet("TsunamiForecastWaveHeight")?.GetString(heightGroup.Key) ?? heightGroup.Key
						));
						foreach (var area in heightGroup) {
							sb.AppendLine(string.Format(
								culture, res.GetStringRequired("TsunamiForecastArea"),
								localFlag ? area.AreaName : SharedResources.TsunamiForecastArea(culture).GetString(area.AreaName) ?? area.AreaName,
								area.ArrivalTime
							));
						}
					}
				}
			}
			return new(culture, title, sb.ToString(), urgentEntry != null ? -5 : 0, "ev") { UrgentEntry = urgentEntry };
		}
	}
}
