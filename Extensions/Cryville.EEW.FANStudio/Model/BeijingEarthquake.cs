using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record BeijingEarthquake(
		[property: JsonPropertyName("eventId")] string EventID,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("depth")] float Depth
	);
}
