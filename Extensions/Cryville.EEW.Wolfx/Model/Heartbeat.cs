using System.Text.Json.Serialization;

namespace Cryville.EEW.Wolfx.Model {
	public record Heartbeat(
		[property: JsonPropertyName("ver")] int ServerVersion,
		[property: JsonPropertyName("id")] string ClientId,
		[property: JsonPropertyName("timestamp")] long Timestamp,
		[property: JsonPropertyName("message")] string ServerMessage
	) : BaseModel;
}
