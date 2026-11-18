using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.NowQuakeCENCInt.Model {
	public class V2Message {
		[JsonPropertyName("schema")]
		public required Schema Schema { get; set; }

		[JsonPropertyName("metadata")]
		public required JsonElement Metadata { get; set; }

		[JsonPropertyName("data")]
		public required JsonElement Data { get; set; }
	}

	public class V2StreamMetadata {
		[JsonPropertyName("type")]
		public required string Type { get; set; }
	}
}
