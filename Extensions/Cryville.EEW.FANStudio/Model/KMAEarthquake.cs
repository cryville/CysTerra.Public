using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record KMAEarthquake(
		[property: JsonPropertyName("id")] string ID,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("createTime")] DateTime CreateTime,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("depth")] float? Depth,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("epiIntensity")] int? EpicenterIntensity,
		[property: JsonPropertyName("placeName")] string PlaceName
	);
}
