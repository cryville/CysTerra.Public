using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record ICLEEW(
		[property: JsonPropertyName("eventId")] string EventID,
		[property: JsonPropertyName("updates")] int Updates,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("depth")] float Depth,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("updateTime")] DateTime UpdateTime,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("insideNet")] int InsideNet,
		[property: JsonPropertyName("sations")] int StationCount,
		[property: JsonPropertyName("sourceType")] string SourceType,
		[property: JsonPropertyName("epiIntensity")] float EpicenterIntensity
	);
}
