using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.Wolfx.Model {
	public record CENCEarthquake(
		[property: JsonPropertyName("type")] string Type,
		string? EventID,
		[property: JsonPropertyName("time")] DateTime Time,
		DateTime? ReportTime,
		[property: JsonPropertyName("location")] string Location,
		[property: JsonPropertyName("placeName")] string RawLocation,
		[property: JsonPropertyName("magnitude")] string Magnitude,
		[property: JsonPropertyName("depth")] string Depth,
		[property: JsonPropertyName("latitude")] string Latitude,
		[property: JsonPropertyName("longitude")] string Longitude,
		[property: JsonPropertyName("intensity")] string Intensity
	);
}
