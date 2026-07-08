using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.ComponentModel;
using Cryville.EEW.TTS;
using Cryville.EEW.Wolfx.Model;
using System.Globalization;

namespace Cryville.EEW.Wolfx.TTS {
	public class CENCEarthquakeTTSMessageGenerator : IContextedGenerator<CENCEarthquake, ITTSMessageGeneratorContext, TTSEntry?>, IPropertiesHolder {
		[LocalizableDisplayName("PNUseRawLocationName")]
		[LocalizableDescription("PDUseRawLocationName")]
		public bool UseRawLocationName { get; set; }

		public TTSEntry? Generate(CENCEarthquake e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(CENCEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			string location = CENCHelpers.ExtractLocationAffixes(UseRawLocationName ? (e.RawLocation ?? e.Location) : e.Location, out var affixes, culture);
			if (
				float.TryParse(e.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
				float.TryParse(e.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon)
			) {
				var locCulture = culture;
				context.NameLocation(lat, lon, Local.Culture, ref culture, out string? name);
				if (name != null) location = name;
			}
			return new(culture, res.GetStringRequired("Title"), string.Format(culture,
				res.GetStringRequired("Body"),
				e.Time, location, e.Magnitude, e.Depth,
				res.GetStringRequired(e.Type == "reviewed" ? "TypeReviewed" : "TypeAutomatic"),
				affixes ?? res.GetStringRequired("DefaultType")
			), 0, e.Type == "reviewed" ? "eq" : "eq_a");
		}
	}
}
