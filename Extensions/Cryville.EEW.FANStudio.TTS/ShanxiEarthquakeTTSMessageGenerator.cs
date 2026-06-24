using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.TTS;
using System.Globalization;

namespace Cryville.EEW.FANStudio.TTS {
	public class ShanxiEarthquakeTTSMessageGenerator : IContextedGenerator<ShanxiEarthquake, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(ShanxiEarthquake e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(ShanxiEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			string location = e.PlaceName;
			var locCulture = culture;
			context.NameLocation(e.Latitude, e.Longitude, Local.Culture, ref culture, out string? name);
			if (name != null) location = name;
			return new(culture, res.GetStringRequired("Title"), string.Format(culture,
				res.GetStringRequired("Body"),
				e.ShockTime, location, e.Magnitude, e.Depth
			), 0, "eq");
		}
	}
}
