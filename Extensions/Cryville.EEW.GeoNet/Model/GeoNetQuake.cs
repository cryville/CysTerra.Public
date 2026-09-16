using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.GeoNet.Model {
	public record GeoNetQuake(
		[property: JsonPropertyName("publicID")] string PublicID,
		[property: JsonPropertyName("time")] DateTimeOffset Time,
		[property: JsonPropertyName("depth")] double Depth,
		[property: JsonPropertyName("magnitude")] double Magnitude,
		[property: JsonPropertyName("mmi")] int MMI,
		[property: JsonPropertyName("locality")] string Locality,
		[property: JsonPropertyName("quality")] string Quality
	);
	public record GeoNetQuakeHistory(
		string PublicID,
		DateTimeOffset Time,
		[property: JsonPropertyName("modificationTime")] DateTimeOffset ModificationTime,
		double Depth,
		double Magnitude,
		int MMI,
		string Locality,
		string Quality
	) : GeoNetQuake(PublicID, Time, Depth, Magnitude, MMI, Locality, Quality);
}
