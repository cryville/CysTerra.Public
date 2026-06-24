using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record SichuanEEW(
		[property: JsonPropertyName("id")] int ID,
		[property: JsonPropertyName("eventId")] string EventID,
		[property: JsonPropertyName("updates")] int Updates,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("createTime")] DateTime CreateTime,
		[property: JsonPropertyName("epiIntensity")] float? EpicenterIntensity,
		[property: JsonPropertyName("infoTypeName")] string InfoTypeName,
		[property: JsonPropertyName("producer")] string Producer
	);
}
