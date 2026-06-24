using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record HKOEarthquake(
		[property: JsonPropertyName("id")] string ID,
		[property: JsonPropertyName("eventId")] string EventID,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("depth")] float Depth,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("placeName")] string PlaceName,
		[property: JsonPropertyName("citystring")] string City,
		[property: JsonPropertyName("region")] string Region,
		[property: JsonPropertyName("verify")] string VerificationStatus
	);
}
