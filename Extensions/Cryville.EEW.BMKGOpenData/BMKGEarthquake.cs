using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cryville.EEW.BMKGOpenData {
	public record BMKGEarthquake(
		[property: JsonPropertyName("Tanggal")] string Date,
		[property: JsonPropertyName("Jam")] string Time,
		[property: JsonPropertyName("DateTime")] DateTimeOffset DateTime,
		[property: JsonPropertyName("Coordinates")] string Coordinates,
		[property: JsonPropertyName("Lintang")] string Latitude,
		[property: JsonPropertyName("Bujur")] string Longitude,
		[property: JsonPropertyName("Magnitude")] string Magnitude,
		[property: JsonPropertyName("Kedalaman")] string Depth,
		[property: JsonPropertyName("Wilayah")] string Region,
		[property: JsonPropertyName("Dirasakan")] string Intensity,
		[property: JsonPropertyName("Potensi")] string Potential,
		[property: JsonPropertyName("Shakemap")] string ShakeMap
	);
	public record BMKGEarthquakeListInfo([property: JsonPropertyName("gempa")] IReadOnlyList<BMKGEarthquake> Earthquakes);
	public record BMKGEarthquakeList([property: JsonPropertyName("Infogempa")] BMKGEarthquakeListInfo EarthquakeInfo);
	public record BMKGEarthquakeLatestInfo([property: JsonPropertyName("gempa")] BMKGEarthquake Earthquake);
	public record BMKGEarthquakeLatest([property: JsonPropertyName("Infogempa")] BMKGEarthquakeLatestInfo EarthquakeInfo);
}
