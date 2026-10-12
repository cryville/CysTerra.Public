using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.NowQuakeCENCInt.Model;
using Cryville.EEW.TTS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cryville.EEW.NowQuakeCENCInt.TTS {
	public class NowQuakeCENCIntDataTTSMessageGenerator : IContextedGenerator<WsData, ITTSMessageGeneratorContext, TTSEntry> {
		public TTSEntry Generate(WsData e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var sb = new StringBuilder();
			sb.AppendLine(res.GetStringRequired("Title"));

			var locCulture = culture;
			string? localLocationName = CENCHelpers.ExtractLocationAffixes(e.Hypocenter, out var affixes, culture);
			if (!context.NameLocation(e.Latitude, e.Longitude, Local.Culture, ref culture, out string? location)) {
				if (localLocationName != null) location = localLocationName;
				else context.NameLocation(e.Latitude, e.Longitude, CultureInfo.InvariantCulture, ref culture, out location);
			}
			sb.AppendFormat(culture,
				res.GetStringRequired("Headline"),
				e.HappenTime, location, e.Magnitude, affixes ?? res.GetStringRequired("DefaultType")
			);
			if (e.Depth is float depth)
				sb.AppendFormat(culture, res.GetStringOrDefault("HeadlineDepth"), e.Depth);
			sb.AppendLine();

			if (e.Stations is IReadOnlyCollection<Station> stations)
				GenerateFromStations(sb, culture, res, stations);
			if (e.MaxIntensity is float maxIntensity && (maxIntensity != 0 || (e.Stations is IReadOnlyCollection<Station> stations2 && stations2.Count > 0)))
				sb.AppendLine(string.Format(culture, res.GetStringRequired("IntensityMax"), maxIntensity));

			return new(culture, res.GetStringRequired("Title"), sb.ToString(), 0, "eq_d");
		}

		const int AREA_COUNT_THRESHOLD = 10;
		static void GenerateFromStations(StringBuilder sb, CultureInfo culture, IMessageStringSet res, IReadOnlyCollection<Station> stations) {
			int count = 0;
			string intName = res.GetStringRequired("Intensity");
			foreach (var areasByIntensity in ExtractStationAreas(stations, culture, res)
				.OrderByDescending(s => s.Intensity)
				.Distinct(AreaEqualityComparer.Instance)
				.GroupBy(s => (int)Math.Round(s.Intensity, MidpointRounding.AwayFromZero))
			) {
				if (count >= AREA_COUNT_THRESHOLD) break;
				string intPrefix = intName;
				if (count == 0) {
					sb.Append(res.GetStringRequired("IntensityIntro"));
					intPrefix = string.Format(culture, res.GetStringRequired("Maximum"), intPrefix);
				}
				else if (areasByIntensity.Key <= 1) break;
				string areaStr = "";
				bool flag = false;
				foreach (var (area, _) in areasByIntensity) {
					if (flag) areaStr += res.GetStringRequired("MediumItemSeparator");
					else flag = true;
					areaStr += area;
					count++;
				}
				sb.AppendLine(string.Format(culture,
					res.GetStringRequired("IntensityArea"),
					intPrefix, areasByIntensity.Key, areaStr
				));
			}
		}

		static IEnumerable<(string Area, float Intensity)> ExtractStationAreas(IEnumerable<Station> stations, CultureInfo culture, IMessageStringSet res) {
			foreach (var station in stations) {
				if (station.LocationName is not LocationName locationName) continue;
				if (locationName.Province is not string province) continue;
				if (locationName.City is not string city) continue;
				yield return (GetAreaName(station.Code, province, city, culture, res), station.Intensity);
			}
		}

		static string GetAreaName(string networkCode, string province, string city, CultureInfo culture, IMessageStringSet res) {
			if (TryGetAreaName(province, city, culture, res, out string candidateResult))
				return candidateResult;
			if (_mapNetworkToProvince.TryGetValue(networkCode, out string? provinceFromNetworkCode) && TryGetAreaName(provinceFromNetworkCode, city, culture, res, out string candidateResult2))
				return candidateResult2;
			return candidateResult;
		}
		static bool TryGetAreaName(string province, string city, CultureInfo culture, IMessageStringSet res, out string result) {
			string provinceKey = province;
			string? provinceName = FindAreaName(ref provinceKey, res.GetStringSetRequired("Provinces"), 2, 3);
			string format = res.GetStringRequired("MacroregionFormat");
			if (provinceName == null) {
				result = string.Format(culture, format, province, city);
				return false;
			}
			var citiesStringSet = res.GetStringSetRequired("Prefectures").GetStringSet(provinceKey);
			if (citiesStringSet == null) {
				result = provinceName;
				return true;
			}
			string cityKey = city;
			string? cityName = FindAreaName(ref cityKey, citiesStringSet, 2, 4);
			if (cityName == null) {
				result = string.Format(culture, format, provinceName, city);
				return false;
			}
			result = string.Format(culture, format, provinceName, cityName);
			return true;
		}

		static string? FindAreaName(ref string key, IMessageStringSet messageStringSet, int minChars, int maxChars) {
			for (int chars = Math.Min(key.Length, maxChars); chars >= minChars; chars--) {
				key = key[..chars];
				if (messageStringSet.GetString(key) is string name) {
					return name;
				}
			}
			return null;
		}

		sealed class AreaEqualityComparer : IEqualityComparer<(string Area, float Intensity)> {
			public static AreaEqualityComparer Instance { get; } = new();
			public bool Equals((string Area, float Intensity) x, (string Area, float Intensity) y) => x.Area == y.Area;
			public int GetHashCode((string Area, float Intensity) obj) => obj.Area.GetHashCode(StringComparison.OrdinalIgnoreCase);
		}

		static readonly Dictionary<string, string> _mapNetworkToProvince = new(StringComparer.OrdinalIgnoreCase) {
			{ "AH", "安徽" },
			{ "BJ", "北京" },
			{ "CQ", "重庆" },
			{ "FJ", "福建" },
			{ "GD", "广东" },
			{ "GS", "甘肃" },
			{ "GX", "广西" },
			{ "GZ", "贵州" },
			{ "HA", "河南" },
			{ "HB", "湖北" },
			{ "HE", "河北" },
			{ "HI", "海南" },
			{ "HL", "黑龙江" },
			{ "HN", "湖南" },
			{ "JL", "吉林" },
			{ "JS", "江苏" },
			{ "JX", "江西" },
			{ "LN", "辽宁" },
			{ "NM", "内蒙古" },
			{ "NX", "宁夏" },
			{ "QH", "青海" },
			{ "SC", "四川" },
			{ "SD", "山东" },
			{ "SH", "上海" },
			{ "SN", "陕西" },
			{ "SX", "山西" },
			{ "TJ", "天津" },
			{ "XJ", "新疆" },
			{ "XZ", "西藏" },
			{ "YN", "云南" },
			{ "ZJ", "浙江" },
		};
	}
}
