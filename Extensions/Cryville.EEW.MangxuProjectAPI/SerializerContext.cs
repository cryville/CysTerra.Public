using Cryville.EEW.MangxuProjectAPI.Model;
using System.Text.Json.Serialization;

namespace Cryville.EEW.MangxuProjectAPI {
	[JsonSerializable(typeof(Message))]
	[JsonSerializable(typeof(ClientMessage))]
	[JsonSourceGenerationOptions(Converters = [typeof(NonstandardDateTimeJsonConverter)], PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower, AllowOutOfOrderMetadataProperties = true)]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
