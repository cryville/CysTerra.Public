using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record CEAEEW(
		[property: JsonPropertyName("id")] string ID,
		[property: JsonPropertyName("eventId")] string EventID,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("updateTime")] DateTime? UpdateTime,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("epiIntensity")] float? EpicenterIntensity,
		[property: JsonPropertyName("depth")] float? Depth,
		[property: JsonPropertyName("updates")] int Updates
	);
}
