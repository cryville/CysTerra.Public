using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cryville.EEW.NowQuakeCENCInt.TTS {
	static class AreaNameHelpers {
		public static string GetAreaName(string networkCode, string province, string city, CultureInfo culture, IMessageStringSet res) {
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

		public static IEqualityComparer<(string Area, double Intensity)> AreaEqualityComparerInstance { get; } = new AreaEqualityComparer();
		sealed class AreaEqualityComparer : IEqualityComparer<(string Area, double Intensity)> {
			public bool Equals((string Area, double Intensity) x, (string Area, double Intensity) y) => x.Area == y.Area;
			public int GetHashCode((string Area, double Intensity) obj) => obj.Area.GetHashCode(StringComparison.OrdinalIgnoreCase);
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
