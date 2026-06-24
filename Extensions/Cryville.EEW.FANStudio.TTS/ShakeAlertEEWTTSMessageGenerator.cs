using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using System;
using System.Globalization;
using System.Text;

namespace Cryville.EEW.FANStudio.TTS {
	public class ShakeAlertEEWTTSMessageGenerator : IContextedGenerator<ShakeAlertEEW, ITTSMessageGeneratorContext, TTSEntry?> {
		readonly ReportUnitStateList _states = new();
		public TTSEntry? Generate(ShakeAlertEEW e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			if (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, Local.TimeZone) >= context.NowcastWarningDelayTolerance) return null;

			using var lres = new LocalizedResource(nameof(ShakeAlertEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var sb = new StringBuilder();
			sb.Append(res.GetStringRequired("Title"));

			var pr = _states.Push(e.ID.ToString(CultureInfo.InvariantCulture), [(int)(e.Magnitude ?? 0)]);

			var locCulture = culture;
			context.NameLocation(e.Latitude, e.Longitude, Local.UnitedStatesCulture, ref locCulture, out string? location);
			location ??= e.PlaceName;

			if (e.Magnitude is float mag) {
				sb.AppendLine(string.Format(culture, res.GetStringRequired("Body"), location, mag.ToString("F1", culture)));
			}
			else {
				sb.AppendLine(string.Format(culture, res.GetStringRequired("BodyNoMagnitude"), location));
			}

			return new(culture, null, sb.ToString(), -100, pr.HasNewMaxState ? pr.MaxState[0] switch {
				<= 3 => "eew_1",
				4 => "eew_2",
				5 => "eew_3",
				6 => "eew_4",
				_ => "eew_5",
			} : "eew_update");
		}
	}
}
