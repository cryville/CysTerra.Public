using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio.TTS {
	public class CWAEEWTTSMessageGenerator : IContextedGenerator<CWAEEW, ITTSMessageGeneratorContext, TTSEntry?> {
		readonly ReportUnitStateList _states = new();
		public TTSEntry? Generate(CWAEEW e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			if (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, Local.TimeZone) >= context.NowcastWarningDelayTolerance) return null;

			using var lres = new LocalizedResource(nameof(CWAEEW), ref culture);
			var res = lres.RootMessageStringSet;
			string id = e.ID.ToString(CultureInfo.InvariantCulture);
			var pr = _states.Push(id, [(int)e.Magnitude]);
			var locCulture = culture;
			return new(culture, null, string.Format(culture,
				res.GetStringRequired("Body"),
				e.Updates,
				context.NameLocation(e.Latitude, e.Longitude, Local.TaiwanCulture, ref locCulture, out string? name) ? name : e.PlaceName,
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
