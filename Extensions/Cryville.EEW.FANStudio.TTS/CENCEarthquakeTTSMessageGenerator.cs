using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.TTS;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio.TTS {
	public class CENCEarthquakeTTSMessageGenerator : IContextedGenerator<CENCEarthquake, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(CENCEarthquake e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(CENCEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			bool reviewedFlag = e.AutoFlag == "I" || e.InfoTypeName.Contains("正式", StringComparison.Ordinal);
			string location = CENCHelpers.ExtractLocationAffixes(e.PlaceName, out var affixes, culture);
			var locCulture = culture;
			context.NameLocation(e.Latitude, e.Longitude, Local.Culture, ref culture, out string? name);
			if (name != null) location = name;
			return new(culture, res.GetStringRequired("Title"), string.Format(culture,
				res.GetStringRequired("Body"),
				e.ShockTime, location, e.Magnitude, e.Depth,
				res.GetStringRequired(reviewedFlag ? "TypeReviewed" : "TypeAutomatic"),
				affixes ?? res.GetStringRequired("DefaultType")
			), 0, reviewedFlag ? "eq" : "eq_a");
		}
	}
}
