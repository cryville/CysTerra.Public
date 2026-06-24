using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record FujianEEW(
		[property: JsonPropertyName("id")] int ID,
		[property: JsonPropertyName("eventId")] string EventID,
		[property: JsonPropertyName("updates")] int Updates,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("sendtime")] DateTime SendTime,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("infoTypeName")] string InfoTypeName
	);
}
