using Cryville.EEW.NowQuakeCENCInt.Model;
using System.Text.Json.Serialization;

namespace Cryville.EEW.NowQuakeCENCInt {
	[JsonSerializable(typeof(WsMessage))]
	[JsonSourceGenerationOptions(Converters = [typeof(NonstandardDateTimeJsonConverter)])]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
