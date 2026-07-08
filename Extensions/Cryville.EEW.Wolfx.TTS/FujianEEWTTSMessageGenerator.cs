using Cryville.Common.Compat;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Globalization;

namespace Cryville.EEW.Wolfx.TTS {
	public class FujianEEWTTSMessageGenerator : IContextedGenerator<FujianEEW, ITTSMessageGeneratorContext, TTSEntry?> {
		readonly ReportUnitStateList _states = new();
		public TTSEntry? Generate(FujianEEW e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			if (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(e.OriginTime, Local.TimeZone) >= context.NowcastWarningDelayTolerance) return null;

			using var lres = new LocalizedResource(nameof(FujianEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var pr = _states.Push(WolfxHelpers.ExtractEventID(e.EventID), [(int)e.Magnitude]);
			var locCulture = culture;
			return new(culture, null, string.Format(culture,
				res.GetStringRequired("Body"),
				e.IsFinal ? res.GetStringRequired("SerialFinal") : string.Format(culture, res.GetStringRequired("Serial"), e.ReportNum),
				context.NameLocation(e.Latitude, e.Longitude, Local.Culture, ref locCulture, out string? name) ? name : e.HypoCenter,
				e.Magnitude
			), -100, pr.HasNewMaxState ? pr.MaxState[0] switch {
				<= 3 => "eew_1",
				4 => "eew_2",
				5 => "eew_3",
				6 => "eew_4",
				_ => "eew_5",
			} : "eew_update");
		}
	}
}
