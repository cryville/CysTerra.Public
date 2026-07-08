using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.Wolfx.Model {
	public record JMAEarthquake(
		string EventId,
		[property: JsonPropertyName("time")] DateTime Time,
		[property: JsonPropertyName("time_full")] DateTime TimeFull,
		[property: JsonPropertyName("location")] string Location,
		[property: JsonPropertyName("magnitude")] string Magnitude,
		[property: JsonPropertyName("shindo")] string Shindo,
		[property: JsonPropertyName("depth")] string Depth,
		[property: JsonPropertyName("latitude")] string Latitude,
		[property: JsonPropertyName("longitude")] string Longitude,
		[property: JsonPropertyName("info")] string TsunamiInfo
	);
}
