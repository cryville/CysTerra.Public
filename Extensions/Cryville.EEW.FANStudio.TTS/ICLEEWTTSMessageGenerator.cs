using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio.TTS {
	public class ICLEEWTTSMessageGenerator : IContextedGenerator<ICLEEW, ITTSMessageGeneratorContext, TTSEntry?> {
		readonly ReportUnitStateList _states = new();
		public TTSEntry? Generate(ICLEEW e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			if (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, Local.TimeZone) >= context.NowcastWarningDelayTolerance) return null;

			using var lres = new LocalizedResource(nameof(ICLEEW), ref culture);
			var res = lres.RootMessageStringSet;

			var pr = _states.Push(e.EventID.ToString(CultureInfo.InvariantCulture), [(int)((e.EpicenterIntensity - 2) / 1.2f)]);

			var locCulture = culture;
			context.NameLocation(e.Latitude, e.Longitude, Local.Culture, ref locCulture, out string? location);
			location ??= e.PlaceName;

			return new(culture, null, string.Format(
				culture, res.GetStringRequired("Body"),
				e.Updates,
				location,
				e.Magnitude.ToString("F1", culture),
				e.EpicenterIntensity
			), -100, pr.HasNewMaxState ? pr.MaxState[0] switch {
				<= 1 => "eew_1",
				2 => "eew_2",
				3 => "eew_3",
				4 => "eew_4",
				_ => "eew_5",
			} : "eew_update");
		}
	}
}
