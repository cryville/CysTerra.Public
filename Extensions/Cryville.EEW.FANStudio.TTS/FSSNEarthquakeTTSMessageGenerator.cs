using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.TTS;
using System.Globalization;

namespace Cryville.EEW.FANStudio.TTS {
	public class FSSNEarthquakeTTSMessageGenerator : IContextedGenerator<FSSNEarthquake, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(FSSNEarthquake e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(FSSNEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			string location = e.PlaceNameZh;
			var locCulture = culture;
			context.NameLocation(e.Latitude, e.Longitude, Local.Culture, ref culture, out string? name);
			if (name != null) location = name;
			return new(culture, res.GetStringRequired("Title"), string.Format(culture,
				res.GetStringRequired("Body"),
				e.ShockTime, location, e.Magnitude, e.Depth.ToString("F0", culture)
			), 0, "eq");
		}
	}
}
