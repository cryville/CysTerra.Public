using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record YunnanEarthquake(
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("latitude")] float? Latitude,
		[property: JsonPropertyName("longitude")] float? Longitude,
		[property: JsonPropertyName("depth")] float? Depth,
		[property: JsonPropertyName("magnitude")] float? Magnitude,
		[property: JsonPropertyName("magnitudel")] float? MagnitudeL,
		[property: JsonPropertyName("placeName")] string? PlaceName
	);
}
