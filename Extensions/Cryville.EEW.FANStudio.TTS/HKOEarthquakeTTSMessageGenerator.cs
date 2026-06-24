using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.TTS;
using System.Globalization;

namespace Cryville.EEW.FANStudio.TTS {
	public class HKOEarthquakeTTSMessageGenerator : IContextedGenerator<HKOEarthquake, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(HKOEarthquake e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(HKOEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			string location = e.Region;
			var locCulture = culture;
			context.NameLocation(e.Latitude, e.Longitude, Local.HongKongCulture, ref culture, out string? name);
			if (name != null) location = name;
			return new(culture, res.GetStringRequired("Title"), string.Format(culture,
				res.GetStringRequired("Body"),
				e.ShockTime, location, e.Magnitude.ToString("F1", culture), e.Depth
			), 0, "eq");
		}
	}
}
