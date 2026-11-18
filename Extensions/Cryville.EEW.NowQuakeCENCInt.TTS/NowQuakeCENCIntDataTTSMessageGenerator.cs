using Cryville.Common.Compat;
using Cryville.EEW.CENC;
using Cryville.EEW.NowQuakeCENCInt.Model;
using Cryville.EEW.TTS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cryville.EEW.NowQuakeCENCInt.TTS {
	[Obsolete("Use the V2 protocol instead.")]
	public class NowQuakeCENCIntDataTTSMessageGenerator : IContextedGenerator<WsData, ITTSMessageGeneratorContext, TTSEntry> {
		public TTSEntry Generate(WsData e, ITTSMessageGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyTTSMessageGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var sb = new StringBuilder();
			sb.AppendLine(res.GetStringRequired("Title"));

			var locCulture = culture;
			string? localLocationName = CENCHelpers.ExtractLocationAffixes(e.Hypocenter, out var affixes, culture);
			string? location;
			if (e.Latitude is double latitude && e.Longitude is double longitude) {
				if (!context.NameLocation(latitude, longitude, Local.Culture, ref culture, out location)) {
					if (localLocationName != null) location = localLocationName;
					else context.NameLocation(latitude, longitude, CultureInfo.InvariantCulture, ref culture, out location);
				}
			}
			else location = localLocationName;
			sb.AppendFormat(culture,
				res.GetStringRequired("Headline"),
				e.HappenTime, location, e.Magnitude, affixes ?? res.GetStringRequired("DefaultType")
			);
			if (e.Depth is double depth)
				sb.AppendFormat(culture, res.GetStringOrDefault("HeadlineDepth"), e.Depth);
			sb.AppendLine();

			if (e.Stations is { } stations)
				GenerateFromStations(sb, culture, res, stations);
			if (e.Maxintensity is double maxIntensity && (maxIntensity != 0 || (e.Stations is { } stations2 && stations2.Count > 0)))
				sb.AppendLine(string.Format(culture, res.GetStringRequired("IntensityMax"), maxIntensity));

			return new(culture, res.GetStringRequired("Title"), sb.ToString(), 0, "eq_d");
		}

		const int AREA_COUNT_THRESHOLD = 10;
		static void GenerateFromStations(StringBuilder sb, CultureInfo culture, IMessageStringSet res, ICollection<Station> stations) {
			int count = 0;
			string intName = res.GetStringRequired("Intensity");
			foreach (var areasByIntensity in ExtractStationAreas(stations, culture, res)
				.OrderByDescending(s => s.Intensity)
				.Distinct(AreaNameHelpers.AreaEqualityComparerInstance)
				.GroupBy(s => (int)Math.Round(s.Intensity, MidpointRounding.AwayFromZero))
			) {
				if (count >= AREA_COUNT_THRESHOLD) break;
				string intPrefix = intName;
				if (count == 0) {
					sb.Append(res.GetStringRequired("IntensityIntro"));
					intPrefix = string.Format(culture, res.GetStringRequired("Maximum"), intPrefix);
				}
				else if (areasByIntensity.Key <= 1) break;
				string areaStr = "";
				bool flag = false;
				foreach (var (area, _) in areasByIntensity) {
					if (flag) areaStr += res.GetStringRequired("MediumItemSeparator");
					else flag = true;
					areaStr += area;
					count++;
				}
				sb.AppendLine(string.Format(culture,
					res.GetStringRequired("IntensityArea"),
					intPrefix, areasByIntensity.Key, areaStr
				));
			}
		}

		static IEnumerable<(string Area, double Intensity)> ExtractStationAreas(IEnumerable<Station> stations, CultureInfo culture, IMessageStringSet res) {
			foreach (var station in stations) {
				if (station.Int is not double @int) continue;
				if (station.LocationName is not LocationName locationName) continue;
				if (locationName.Province is not string province) continue;
				if (locationName.City is not string city) continue;
				if (city.Contains("直辖", StringComparison.Ordinal)) {
					if (locationName.County is not string county)
						continue;
					city = county;
				}
				yield return (AreaNameHelpers.GetAreaName(station.Code, province, city, culture, res), @int);
			}
		}
	}
}
