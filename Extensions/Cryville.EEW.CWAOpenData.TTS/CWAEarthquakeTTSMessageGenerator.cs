using Cryville.Common.Compat;
using Cryville.EEW.CWA;
using Cryville.EEW.CWAOpenData.Model;
using Cryville.EEW.TTS;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Cryville.EEW.CWAOpenData.TTS {
	public partial class CWAEarthquakeTTSMessageGenerator : IContextedGenerator<Earthquake, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(Earthquake e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			var sb = new StringBuilder();
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			bool localFlag = culture.Equals(Local.Culture);
			var title = localFlag ? e.ReportType : res.GetStringSet("Title")?.GetString(e.ReportType) ?? "";
			sb.AppendLine(title);
			if (e.EarthquakeInfo is EarthquakeInfo info) {
				if (localFlag) {
					sb.AppendLine(BadDateTimeRegex().Replace(CWAMessageUtils.ToHalfwidthDigits(e.ReportContent), info.OriginTime.ToString("M月d日 HH:mm，", culture)));
				}
				else {
					var locCulture = culture;
					var epicenter = info.Epicenter;
					CWAOpenDataUtils.GetCoordinates(epicenter, out float lat, out float lon);
					sb.AppendLine(string.Format(
						culture, res.GetStringRequired("EarthquakeBody"),
						TimeZoneInfo.ConvertTime(info.OriginTime, Local.TimeZone).DateTime,
						context.NameLocation(lat, lon, Local.Culture, ref locCulture, out string? name) ? name : epicenter.Location,
						info.EarthquakeMagnitude.MagnitudeValue,
						CWAMessageUtils.ToLongDisplayIntensity(CWAEarthquakeReportGenerator.FindMaxIntensity(e.Intensity), culture)
					));
				}
				sb.Append(string.Format(culture, res.GetStringRequired("EarthquakeFocalDepth"), info.FocalDepth));
			}
			return new(culture, title, sb.ToString(), 0, "eq_d");
		}
#if NET7_0_OR_GREATER
		[GeneratedRegex(@"^[\d\/\-\:]+")]
		private static partial Regex BadDateTimeRegex();
#else
		static readonly Regex r_BadDateTimeRegex = new(@"^[\d\/\-\:]+");
		static Regex BadDateTimeRegex() => r_BadDateTimeRegex;
#endif
	}
}
