using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record USGSEarthquake(
		[property: JsonPropertyName("id")] string ID,
		[property: JsonPropertyName("title")] string Title,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("updateTime")] DateTime UpdateTime,
		[property: JsonPropertyName("longitude")] double Longitude,
		[property: JsonPropertyName("latitude")] double Latitude,
		[property: JsonPropertyName("depth")] double Depth,
		[property: JsonPropertyName("url")] Uri URL
	);
}
