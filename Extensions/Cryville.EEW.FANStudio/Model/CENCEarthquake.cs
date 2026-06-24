using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record CENCEarthquake(
		[property: JsonPropertyName("id")] long ID,
		[property: JsonPropertyName("eventId")] string EventID,
		[property: JsonPropertyName("autoFlag")] string AutoFlag,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("createTime")] DateTime CreateTime,
		[property: JsonPropertyName("depth")] float Depth,
		[property: JsonPropertyName("earthtype")] int EarthquakeType,
		[property: JsonPropertyName("infoTypeName")] string InfoTypeName
	);
}
