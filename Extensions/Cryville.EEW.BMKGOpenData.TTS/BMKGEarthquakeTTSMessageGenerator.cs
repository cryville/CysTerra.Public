using Cryville.Common.Compat;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using System;
using System.Globalization;
using System.Text;

namespace Cryville.EEW.BMKGOpenData.TTS {
	public class BMKGEarthquakeTTSMessageGenerator : IContextedGenerator<BMKGEarthquake, ITTSMessageGeneratorContext, TTSEntry> {
		public TTSEntry Generate(BMKGEarthquake e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			var sb = new StringBuilder();
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			bool localFlag = culture.Equals(Local.Culture);

			sb.AppendLine(res.GetStringRequired("Title"));
			sb.AppendLine(string.Format(
				culture, res.GetStringRequired("Headline"),
				TimeZoneInfo.ConvertTime(e.DateTime, Local.TimeZone).DateTime,
				float.TryParse(e.Magnitude, NumberStyles.Float, CultureInfo.InvariantCulture, out float mag) ? string.Format(culture, res.GetStringRequired("HeadlineMagnitude"), mag) : null,
				BMKGHelpers.ExtractDepth(e.Depth) is float depth ? string.Format(culture, res.GetStringRequired("HeadlineDepth"), depth) : null
			));
			if (localFlag) {
				if (e.Region.StartsWith("Pusat gempa", StringComparison.OrdinalIgnoreCase))
					sb.AppendLine(string.Format(culture, "{0}.", e.Region));
				else
					sb.AppendLine(string.Format(culture, res.GetStringRequired("Region"), e.Region));
			}
			else if (BMKGHelpers.ExtractCoordinates(e.Coordinates, out float lat, out float lon)) {
				var locCulture = culture;
				sb.AppendLine(string.Format(
					culture, res.GetStringRequired("Region"),
					context.NameLocation(lat, lon, Local.Culture, ref locCulture, out string? name) ? name : BMKGHelpers.NormalizeLocation(e.Region)
				));
			}
			if (e.Intensity is string intensity && BMKGHelpers.GetMaxIntensity(intensity) is string maxIntensity && !string.IsNullOrEmpty(maxIntensity))
				sb.AppendLine(string.Format(culture, res.GetStringRequired("MaxIntensity"), RomanNumerals.RomanToInteger(maxIntensity)));
			return new TTSEntry(culture, res.GetStringRequired("TitleStandAlone"), sb.ToString(), 0, e.Intensity != null ? "eq_d" : "eq");
		}
	}
}
