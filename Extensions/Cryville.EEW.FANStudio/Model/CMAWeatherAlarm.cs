using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record CMAWeatherAlarm(
		[property: JsonPropertyName("id")] string ID,
		[property: JsonPropertyName("headline")] string Headline,
		[property: JsonPropertyName("effective")] DateTime EffectiveTime,
		[property: JsonPropertyName("description")] string Description,
		[property: JsonPropertyName("longitude")] float? Longitude,
		[property: JsonPropertyName("latitude")] float? Latitude,
		[property: JsonPropertyName("type")] string Type,
		[property: JsonPropertyName("title")] string Title
	);
}
