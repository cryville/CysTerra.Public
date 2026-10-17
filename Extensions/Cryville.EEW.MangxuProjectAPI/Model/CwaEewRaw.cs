using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.MangxuProjectAPI.Model {
	public record CwaEewRaw(
		[property: JsonPropertyName("payload")] CwaEewRawPayload Payload,
		[property: JsonPropertyName("received_time_utc")] DateTime ReceivedTimeUtc,
		[property: JsonPropertyName("topic")] string Topic
	);
	public record CwaEewRawPayload(
		[property: JsonPropertyName("depth")] float Depth,
		[property: JsonPropertyName("description")] string Description,
		[property: JsonPropertyName("epicenterLat")] float EpicenterLat,
		[property: JsonPropertyName("epicenterLon")] float EpicenterLon,
		[property: JsonPropertyName("identifier")] string Identifier,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("number")] int Number,
		[property: JsonPropertyName("originTime")] long OriginTime,
		[property: JsonPropertyName("pgaAdj")] float PgaAdj,
		[property: JsonPropertyName("sendTime")] long SendTime
	);
}
