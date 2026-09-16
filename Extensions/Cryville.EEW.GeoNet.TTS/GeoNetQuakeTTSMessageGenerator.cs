using Cryville.Common.Compat;
using Cryville.EEW.GeoJSON;
using Cryville.EEW.GeoNet.Model;
using Cryville.EEW.TTS;
using System;
using System.Globalization;
using System.Text;

namespace Cryville.EEW.GeoNet.TTS {
	public class GeoNetQuakeTTSMessageGenerator : IContextedGenerator<Feature<GeoNetQuake>, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(Feature<GeoNetQuake> e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			return Generate(e.Geometry as Point, e.Properties, context, ref culture);
		}

		internal static TTSEntry? Generate(Point? point, GeoNetQuake e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			if (e.Quality != "best") return null;

			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var sb = new StringBuilder();
			var locCulture = culture;
			string location = e.Locality;
			if (point?.Coordinates is Position coords && context.NameLocation(coords.Latitude, coords.Longitude, Local.Culture, ref locCulture, out string? name) && name != null) {
				location = name;
			}

			sb.Append(string.Format(culture,
				res.GetStringRequired("BodyQuake"),
				TimeZoneInfo.ConvertTime(e.Time, Local.TimeZone).DateTime,
				e.Magnitude.ToString("F1", culture),
				location,
				e.Depth.ToString("F1", culture)
			));
			if (e.MMI > 0) {
				sb.Append(string.Format(culture, res.GetStringRequired("MaxIntensity"), e.MMI));
			}
			sb.AppendLine();

			return new(culture, res.GetStringRequired("TitleQuake"), sb.ToString(), 0, "eq"); // Only one sound effect for the "best" quality
		}
	}
	public class GeoNetQuakeHistoryTTSMessageGenerator : IContextedGenerator<Feature<GeoNetQuakeHistory>, ITTSMessageGeneratorContext, TTSEntry?> {
		public TTSEntry? Generate(Feature<GeoNetQuakeHistory> e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			return GeoNetQuakeTTSMessageGenerator.Generate(e.Geometry as Point, e.Properties, context, ref culture);
		}
	}
}
