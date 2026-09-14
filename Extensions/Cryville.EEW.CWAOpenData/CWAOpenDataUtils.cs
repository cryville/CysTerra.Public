using Cryville.Common.Compat;
using Cryville.EEW.CWAOpenData.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cryville.EEW.CWAOpenData {
	public static partial class CWAOpenDataUtils {
#if NET7_0_OR_GREATER
		[GeneratedRegex(@"\(位於(.*?)\)")]
		private static partial Regex ShortLocationRegex();
#else
		static readonly Regex r_ShortLocationRegex = new(@"\(位於(.*?)\)");
		static Regex ShortLocationRegex() => r_ShortLocationRegex;
#endif
		public static string? ToShortLocation(string? location) {
			if (location is null) return null;
			var m = ShortLocationRegex().Match(location);
			if (m.Success) return m.Groups[1].Value;
			return location;
		}

		public static void GetCoordinates(Epicenter epicenter, out float lat, out float lon) {
			ThrowHelper.ThrowIfNull(epicenter);
			lat = epicenter.EpicenterLatitude;
			lon = epicenter.EpicenterLongitude;
			if (lon >= 180) lon -= 360;
		}

#if NET7_0_OR_GREATER
		[GeneratedRegex(@"([\d\.]+)\s*([^\d\.]+)")]
		private static partial Regex WaveHeightComponent();
#else
		static readonly Regex r_WaveHeightComponent = new(@"([\d\.]+)\s*([^\s\d\.]+)");
		static Regex WaveHeightComponent() => r_WaveHeightComponent;
#endif
		public static float? ToWaveHeight(string text) {
			if (string.IsNullOrWhiteSpace(text)) return null;
			float result = 0;
			foreach (var match in WaveHeightComponent().Matches(text).Cast<Match>()) {
				result += (float.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var comp) ? comp : 0) * match.Groups[2].Value switch {
					"公分" => 1e-2f,
					"公尺" => 1,
					_ => 0,
				};
			}
			return MathF.Round(result, 2, MidpointRounding.AwayFromZero);
		}

		public static float GetTsunamiWarningSeverity(WarningArea area) => area?.WaveHeight switch {
			"大於6公尺" => 1.5f,
			"3至6公尺" => 1.25f,
			"1至3公尺" => 1,
			"小於1公尺" or
			"0.3至1公尺" => 0.75f,
			"小於0.3公尺" => 0.5f,
			_ => -1,
		};

		public static int KeyTsunamiForecastWaveHeight(string heightString) => heightString switch {
			"大於6公尺" => 4,
			"3至6公尺" => 3,
			"1至3公尺" => 2,
			"小於1公尺" or
			"0.3至1公尺" => 1,
			"小於0.3公尺" => 0,
			_ => -1,
		};
	}
}
