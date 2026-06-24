using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using Cryville.EEW.TTS;
using System;
using System.Globalization;
using System.Text;

namespace Cryville.EEW.FANStudio.TTS {
	public class FujianEEWTTSMessageGenerator : IContextedGenerator<FujianEEW, ITTSMessageGeneratorContext, TTSEntry?> {
		readonly ReportUnitStateList _states = new();
		public TTSEntry? Generate(FujianEEW e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			if (DateTime.UtcNow - TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, Local.TimeZone) >= context.NowcastWarningDelayTolerance) return null;

			using var lres = new LocalizedResource(nameof(FujianEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var sb = new StringBuilder();
			sb.Append(res.GetStringRequired("Title"));

			string id = CENCHelpers.ExtractEventID(e.EventID, out int? revision);
			if (revision != null) {
				sb.Append(string.Format(culture, res.GetStringRequired("Revision"), revision));
			}

			var pr = _states.Push(e.EventID.ToString(CultureInfo.InvariantCulture), [(int)e.Magnitude]);

			var locCulture = culture;
			context.NameLocation(e.Latitude, e.Longitude, Local.Culture, ref locCulture, out string? location);
			location ??= e.PlaceName;

			sb.AppendLine(string.Format(culture, res.GetStringRequired("Body"), location, e.Magnitude));

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
