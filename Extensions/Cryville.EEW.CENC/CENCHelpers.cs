using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Cryville.EEW.CENC {
	public static partial class CENCHelpers {
		[return: NotNullIfNotNull(nameof(id))]
		public static string? ExtractEventID(string? id, out int? revision) {
			revision = null;
			if (id == null) return null;
			int index = id.IndexOf('_', StringComparison.Ordinal);
			if (index == -1) return id;
			if (int.TryParse(id[(index + 1)..], NumberStyles.Integer, CultureInfo.InvariantCulture, out int rev))
				revision = rev;
			return id[..index];
		}

		static readonly string[] _localAreas = [
			"北京", "天津", "河北", "山西", "内蒙古",
			"辽宁", "吉林", "黑龙江",
			"上海", "江苏", "浙江", "安徽", "福建", "江西", "山东",
			"河南", "湖北", "湖南", "广东", "广西", "海南",
			"重庆", "四川", "贵州", "云南", "西藏",
			"陕西", "甘肃", "青海", "宁夏", "新疆",
			"香港", "澳门", "台湾",
		];
		public static int GetSpecificity(string? location) {
			if (location is null) return 0;
			if (location.StartsWith("台湾", StringComparison.Ordinal)) return 4;
			foreach (var area in _localAreas) {
				if (location.StartsWith(area, StringComparison.Ordinal)) {
					return 6;
				}
			}
			return 3;
		}

#if NET7_0_OR_GREATER
		[GeneratedRegex(@"\p{Ps}(.*?)\p{Pe}")]
		private static partial Regex LocationAffixRegex();
#else
		static readonly Regex r_LocationAffixRegex = new(@"\p{Ps}(.*?)\p{Pe}");
		static Regex LocationAffixRegex() => r_LocationAffixRegex;
#endif
		[MethodImpl(MethodImplOptions.NoInlining)]
		[return: NotNullIfNotNull(nameof(location))]
		public static string? ExtractLocationAffixes(string? location, out string? affixes, CultureInfo culture) {
			if (location == null) {
				affixes = null;
				return null;
			}
			var lres = new LocalizedResource("", culture);
			var res = lres.RootMessageStringSet.GetStringSetRequired("LocationAffixes");
			List<string> convertedAffixList = [];
			string result = LocationAffixRegex().Replace(location, m => {
				string affix = m.Groups[1].Value;
				string? convertedAffix = null;
				if (affix.Contains("有感", StringComparison.Ordinal) || affix.Contains('正', StringComparison.Ordinal)) {
					convertedAffix = ""; // 有感, 更正, 修正
				}
				else if (affix.Contains('叠', StringComparison.Ordinal)) {
					convertedAffix = ""; // 叠加地震
				}
				else if (affix.Contains('塌', StringComparison.Ordinal) || affix.Contains('陷', StringComparison.Ordinal)) {
					convertedAffix = res.GetString("Collapse"); // 塌陷
				}
				else if (affix.Contains('爆', StringComparison.Ordinal)) {
					convertedAffix = res.GetString("Explosion"); // 爆破
				}
				else if (affix.Contains('矿', StringComparison.Ordinal)) {
					convertedAffix = res.GetString("Mining"); // 矿震
				}
				if (!string.IsNullOrEmpty(convertedAffix)) {
					if (affix.Contains('疑', StringComparison.Ordinal))
						convertedAffix = string.Format(culture, res.GetStringRequired("Suspected"), convertedAffix);
					convertedAffixList.Add(convertedAffix);
				}
				return convertedAffix != null ? "" : m.Value;
			});
			affixes = convertedAffixList.Count != 0 ? string.Join(res.GetStringRequired("Separator"), convertedAffixList) : null;
			return result;
		}
	}
}
