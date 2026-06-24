using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Cryville.EEW.CWA {
	public static partial class CWAMessageUtils {
		public static int KeyIntensity(string? intensity) => intensity switch {
			"1級" => 2,
			"2級" => 4,
			"3級" => 6,
			"4級" => 8,
			"5級" or "5弱" => 10,
			"5強" => 11,
			"6級" or "6弱" => 12,
			"6強" => 13,
			"7級" => 14,
			_ => -1,
		};
		public static string? ToLongDisplayIntensity(string? intensity, CultureInfo culture) {
			using var lres = new LocalizedResource("", culture);
			var res = lres.RootMessageStringSet;
			if (intensity is null) return res.GetStringRequired("IntensityUnknown");
			return intensity
				.Replace("級", res.GetStringRequired("IntensitySuffixLevel"), StringComparison.Ordinal)
				.Replace("弱", res.GetStringRequired("IntensitySuffixLower"), StringComparison.Ordinal)
				.Replace("強", res.GetStringRequired("IntensitySuffixUpper"), StringComparison.Ordinal);
		}
		public static string? ToShortDisplayIntensity(string? intensity) {
			if (intensity is null) return null;
			return intensity
				.Replace("級", "", StringComparison.Ordinal)
				.Replace("弱", "\u02d7", StringComparison.Ordinal)
				.Replace("強", "\u02d6", StringComparison.Ordinal);
		}

#if NET7_0_OR_GREATER
		[GeneratedRegex(@"[\uff0e-\uff19]")]
		private static partial Regex HalfwidthDigitRegex();
#else
		static readonly Regex r_HalfwidthDigitRegex = new(@"[\uff0e-\uff19]");
		static Regex HalfwidthDigitRegex() => r_HalfwidthDigitRegex;
#endif
		public static string ToHalfwidthDigits(string reportContent) {
			return HalfwidthDigitRegex().Replace(reportContent, m => ((char)(m.Value[0] & 0xff | 0x20)).ToString()).Replace('\ufe52', '.');
		}
	}
}
