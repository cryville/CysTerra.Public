using Cryville.Common.Compat;
using Cryville.EEW.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Cryville.EEW.BMKGOpenData {
	public static partial class BMKGHelpers {
#if NET7_0_OR_GREATER
		[GeneratedRegex(@"^(?:([IVX]+|\d+)(?:\s*-\s*([IVX]+|\d+))?\s+)?(.+?)$")]
		private static partial Regex IntensityRegex();
		[GeneratedRegex(@"^(.+?)(?:\s+([IVX]+|\d+)(?:\s*-\s*([IVX]+|\d+))?)?$")]
		private static partial Regex IntensityAltRegex();
#else
		static readonly Regex r_IntensityRegex = new(@"^(?:([IVX]+|\d+)(?:\s*-\s*([IVX]+|\d+))?\s+)?(.+?)$");
		static Regex IntensityRegex() => r_IntensityRegex;

		static readonly Regex r_IntensityAltRegex = new(@"^(.+?)(?:\s+([IVX]+|\d+)(?:\s*-\s*([IVX]+|\d+))?)?$");
		static Regex IntensityAltRegex() => r_IntensityAltRegex;
#endif
		public static IEnumerable<(string, string, string)> ExtractIntensity(string intensity) {
			if (intensity == null) yield break;
			string minIntensity = "", maxIntensity = "";
			foreach (var i in intensity.Split(',')) {
				string input = i.Trim();

				var match = IntensityRegex().Match(input);
				if (!match.Groups[1].Success) {
					var matchAlt = IntensityAltRegex().Match(input);
					if (matchAlt.Groups[2].Success) {
						yield return ExtractFromMatch(matchAlt.Groups[2], matchAlt.Groups[3], matchAlt.Groups[1], ref minIntensity, ref maxIntensity);
					}
				}
				yield return ExtractFromMatch(match.Groups[1], match.Groups[2], match.Groups[3], ref minIntensity, ref maxIntensity);

				static (string, string, string) ExtractFromMatch(Group minIntGroup, Group maxIntGroup, Group nameGroup, ref string minIntensity, ref string maxIntensity) {
					if (minIntGroup.Success) {
						minIntensity = IntensityToRoman(minIntGroup.Value);
						if (maxIntGroup.Success) {
							maxIntensity = IntensityToRoman(maxIntGroup.Value);
						}
						else {
							maxIntensity = minIntensity;
						}
					}
					return (minIntensity, maxIntensity, nameGroup.Value);
				}
				static string IntensityToRoman(string intensity) =>
					int.TryParse(intensity, NumberStyles.Integer, CultureInfo.InvariantCulture, out int num) ? RomanNumerals.IntegerToRoman(num) : intensity;
			}
		}
		public static string? GetMaxIntensity(string intensity) => ExtractIntensity(intensity)
			.Select(i => i.Item2)
			.OrderByDescending(RomanNumerals.RomanToInteger)
			.FirstOrDefault();

#if NET7_0_OR_GREATER
		[GeneratedRegex(@"^\s*Pusat\s*gempa\s*berada\s*di\s*(laut|darat)?,?\s*(.+)$", RegexOptions.IgnoreCase)]
		private static partial Regex LocationRegex();
#else
		static readonly Regex r_LocationRegex = new(@"^\s*Pusat\s*gempa\s*berada\s*di\s*(laut|darat)?,?\s*(.+)$", RegexOptions.IgnoreCase);
		static Regex LocationRegex() => r_LocationRegex;
#endif
		[return: NotNullIfNotNull(nameof(location))]
		public static string? NormalizeLocation(string? location) {
			if (location == null) return null;
			var m = LocationRegex().Match(location);
			if (!m.Success) return location;
			return m.Groups[2].Value;
		}

		public static bool ExtractCoordinates(string coordinates, out float lat, out float lon) {
			ThrowHelper.ThrowIfNull(coordinates);
			var index = coordinates.IndexOf(',', StringComparison.Ordinal);
			Unsafe.SkipInit(out lon);
			return float.TryParse(coordinates.AsSpan()[..index], NumberStyles.Float, CultureInfo.InvariantCulture, out lat)
				&& float.TryParse(coordinates.AsSpan()[(index + 1)..], NumberStyles.Float, CultureInfo.InvariantCulture, out lon);
		}
		public static float? ExtractDepth(string depth) {
			ThrowHelper.ThrowIfNull(depth);
			return float.TryParse(depth.AsSpan()[..depth.IndexOf(' ', StringComparison.Ordinal)], NumberStyles.Float, CultureInfo.InvariantCulture, out float result) ? result : null;
		}
	}
}
