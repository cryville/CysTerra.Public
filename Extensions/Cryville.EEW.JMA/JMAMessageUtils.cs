using System;
using System.Globalization;

namespace Cryville.EEW.JMA {
	public static class JMAMessageUtils {
		public static string? ToShortDisplayShindo(string? shindo) {
			if (shindo is null) return null;
			if (shindo == "震度５弱以上未入電") return "5*";
			return shindo.Replace('-', '\u02d7').Replace('+', '\u02d6');
		}

		public static string ToLongDisplayShindo(string? shindo, CultureInfo culture) {
			using var lres = new LocalizedResource("", culture);
			var res = lres.RootMessageStringSet;
			if (shindo is null) return res.GetStringRequired("Unknown");
			return shindo
				.Replace("-", res.GetStringRequired("IntensitySuffixLower"), StringComparison.Ordinal)
				.Replace("+", res.GetStringRequired("IntensitySuffixUpper"), StringComparison.Ordinal);
		}

		public static int KeyTsunamiWarning(string code) => code switch {
			"52" or "53" => 3,
			"51" => 2,
			"62" => 1,
			"71" or "72" or "73" => 0,
			_ => -1,
		};

		public static int GetEpicenterAreaSpecificity(string? code) => int.TryParse(code, NumberStyles.Integer, CultureInfo.InvariantCulture, out var areaCode) ? areaCode switch {
			< 900 or (>= 902 and <= 904) or (>= 911 and <= 920) => 5,
			< 939 => 3,
			< 999 => 1,
			_ => 0,
		} : 0;

		public static float GetVolcanicWarningSeverity(string code) => code switch {
			"11" or "21" or "32" or "33" or "35" => 0f,
			"12" or "22" => 0.75f,
			"13" or "23" or "36" => 0.875f,
			"14" or "24" or "31" => 1f,
			"15" or "25" => 1.25f,
			_ => 1f,
		};
	}
}
