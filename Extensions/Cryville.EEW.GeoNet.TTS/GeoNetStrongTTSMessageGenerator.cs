using Cryville.Common.Compat;
using Cryville.EEW.GeoNet.Model;
using Cryville.EEW.TTS;
using System.Globalization;
using System.Linq;

namespace Cryville.EEW.GeoNet.TTS {
	public class GeoNetStrongTTSMessageGenerator : IContextedGenerator<GeoNetStrong, ITTSMessageGeneratorContext, TTSEntry> {
		public TTSEntry Generate(GeoNetStrong e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var m = e.Metadata;

			var locCulture = culture;
			string location = m.Description;
			if (context.NameLocation(m.Latitude, m.Longitude, Local.Culture, ref locCulture, out string? name) && name != null) {
				location = name;
			}

			return new(
				culture,
				res.GetStringRequired("TitleStrong"),
				string.Format(culture,
					res.GetStringRequired("BodyStrong"),
					m.Magnitude.ToString("F1", culture),
					location,
					m.Depth.ToString("F1", culture),
					e.Features.Max(f => f.Properties.MMI)
				),
				0,
				"eq_d"
			);
		}
	}
}
