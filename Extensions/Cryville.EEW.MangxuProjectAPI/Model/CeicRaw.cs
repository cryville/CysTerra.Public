using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Cryville.EEW.MangxuProjectAPI.Model {
	[SuppressMessage("Design", "CA1054", Justification = "Malformed URL")]
	[SuppressMessage("Design", "CA1056", Justification = "Malformed URL")]
	public record CeicRaw(
		[property: JsonPropertyName("depth")] int Depth,
		[property: JsonPropertyName("determination_type")] string DeterminationType,
		[property: JsonPropertyName("eq_type")] string EqType,
		[property: JsonPropertyName("eqid")] string Eqid,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("loc_name")] string LocName,
		[property: JsonPropertyName("loc_province")] string LocProvince,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("mag")] float Mag,
		[property: JsonPropertyName("ring")] bool Ring,
		[property: JsonPropertyName("time")] long Time,
		[property: JsonPropertyName("updateTime")] long UpdateTime,
		[property: JsonPropertyName("url")] string Url
	);
}
