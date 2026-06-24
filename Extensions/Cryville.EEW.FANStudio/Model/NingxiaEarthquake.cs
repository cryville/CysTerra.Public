using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record NingxiaEarthquake(
		[property: JsonPropertyName("id")] string ID,
		[property: JsonPropertyName("title")] string Title,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("depth")] float Depth,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("magnitude")] float Magnitude
	);
}
