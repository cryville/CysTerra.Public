using Cryville.Common.Compat;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Globalization;

namespace Cryville.EEW.Wolfx.TTS {
	public class CENCEEWTTSMessageGenerator : IContextedGenerator<CENCEEW, ITTSMessageGeneratorContext, TTSEntry?> {
		readonly ReportUnitStateList _states = new();
		public TTSEntry? Generate(CENCEEW e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			if (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(e.OriginTime, Local.TimeZone) >= context.NowcastWarningDelayTolerance) return null;

			using var lres = new LocalizedResource(nameof(CENCEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var pr = _states.Push(WolfxHelpers.ExtractEventID(e.EventID), [(int)((e.MaxIntensity - 2) / 1.2f)]);
			var locCulture = culture;
			return new(culture, null, string.Format(
				culture, res.GetStringRequired("Body"),
				e.ReportNum, context.NameLocation(e.Latitude, e.Longitude, Local.Culture, ref locCulture, out string? name) ? name : e.HypoCenter,
				e.Magnitude,
				e.MaxIntensity
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
