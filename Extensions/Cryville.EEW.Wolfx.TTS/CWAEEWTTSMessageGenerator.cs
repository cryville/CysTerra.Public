using Cryville.Common.Compat;
using Cryville.EEW.CWA;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Globalization;

namespace Cryville.EEW.Wolfx.TTS {
	public class CWAEEWTTSMessageGenerator : IContextedGenerator<CWAEEW, ITTSMessageGeneratorContext, TTSEntry?> {
		readonly ReportUnitStateList _states = new();
		public TTSEntry? Generate(CWAEEW e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			if (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(e.OriginTime, Local.TaiwanTimeZone) >= context.NowcastWarningDelayTolerance) return null;

			using var lres = new LocalizedResource(nameof(CWAEEW), ref culture);
			var res = lres.RootMessageStringSet;
			string id = e.ID.ToString(CultureInfo.InvariantCulture);
			if (e.IsCancellation) {
				_states.Invalidate(id);
				return new(culture, null, res.GetStringRequired("Cancel"), -110, "eew_update_cancel");
			}
			var pr = _states.Push(id, [int.TryParse(e.MaxIntensity?[0].ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var i0) ? i0 : 0]);
			var locCulture = culture;
			return new(culture, null, string.Format(culture,
				res.GetStringRequired("Body"),
				e.ReportNum,
				context.NameLocation(e.Latitude, e.Longitude, Local.TaiwanCulture, ref locCulture, out string? name) ? name : e.HypoCenter,
				CWAMessageUtils.ToLongDisplayIntensity(e.MaxIntensity, culture)
			), -100, pr.HasNewMaxState ? pr.MaxState[0] switch {
				1 or
				2 => "eew_1",
				3 => "eew_2",
				4 => "eew_3",
				5 => "eew_4",
				6 or
				7 => "eew_5",
				_ => "eew_1",
			} : "eew_update");
		}
	}
}
