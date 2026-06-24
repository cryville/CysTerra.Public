using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using System;
using System.Globalization;
using System.Text;

namespace Cryville.EEW.FANStudio.TTS {
	public class CEAEEWTTSMessageGenerator : IContextedGenerator<CEAEEW, ITTSMessageGeneratorContext, TTSEntry?> {
		readonly ReportUnitStateList _states = new();
		public TTSEntry? Generate(CEAEEW e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			if (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, Local.TimeZone) >= context.NowcastWarningDelayTolerance) return null;

			using var lres = new LocalizedResource(nameof(CEAEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var sb = new StringBuilder();

			var pr = _states.Push(e.EventID, [(int)(e.EpicenterIntensity is float epiIntensity ? ((epiIntensity - 2) / 1.2f) : (e.Magnitude - 2))]);

			var locCulture = culture;
			context.NameLocation(e.Latitude, e.Longitude, Local.Culture, ref locCulture, out string? location);
			location ??= e.PlaceName;

			sb.AppendFormat(
				culture, res.GetStringRequired("Body"),
				e.Updates,
				location,
				e.Magnitude.ToString("F1", culture)
			);
			if (e.EpicenterIntensity is float epiIntensity2) {
				sb.AppendFormat(culture, res.GetStringRequired("MaxIntensity"), epiIntensity2);
			}

			return new(culture, null, sb.ToString(), -100, pr.HasNewMaxState ? pr.MaxState[0] switch {
				<= 1 => "eew_1",
				2 => "eew_2",
				3 => "eew_3",
				4 => "eew_4",
				_ => "eew_5",
			} : "eew_update");
		}
	}
}
