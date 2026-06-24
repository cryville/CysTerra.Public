using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.TTS;
using System;
using System.Globalization;
using System.Text;

namespace Cryville.EEW.FANStudio.TTS {
	public class KMAEarthquakeTTSMessageGenerator : IContextedGenerator<KMAEarthquake, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(KMAEarthquake e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(KMAEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			var sb = new StringBuilder();
			string location = e.PlaceName;
			var locCulture = culture;
			context.NameLocation(e.Latitude, e.Longitude, Local.SouthKoreaCulture, ref culture, out string? name);
			if (name != null) location = name;
			sb.AppendFormat(
				culture, res.GetStringRequired("Body"),
				e.ShockTime,
				location,
				e.Magnitude.ToString("F1", culture)
			);
			if (e.Depth is float depth) {
				sb.AppendFormat(culture, res.GetStringRequired("Depth"), e.Depth);
			}
			int? maxIntensity = e.EpicenterIntensity;
			if (maxIntensity is int maxIntensityValue)
				sb.AppendFormat(culture, res.GetStringRequired("MaxIntensity"), maxIntensityValue);
			return new(culture, res.GetStringRequired("Title"), sb.ToString(), 0, "eq");
		}
	}
}
