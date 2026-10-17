using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.MangxuProjectAPI.Model {
	public record CeaData(
		[property: JsonPropertyName("depth")] float? Depth,
		[property: JsonPropertyName("epiIntensity")] float? EpiIntensity,
		[property: JsonPropertyName("eventId")] string EventId,
		[property: JsonPropertyName("event_time")] long EventTime,
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("province")] string? Province,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("signature")] string? Signature,
		[property: JsonPropertyName("source")] string? Source,
		[property: JsonPropertyName("updates")] int Updates
	);
}
