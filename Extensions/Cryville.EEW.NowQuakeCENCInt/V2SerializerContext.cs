using Cryville.EEW.GeoJSON;
using Cryville.EEW.NowQuakeCENCInt.Model;
using System.Text.Json.Serialization;

namespace Cryville.EEW.NowQuakeCENCInt {
	[JsonSerializable(typeof(V2Message))]
	[JsonSerializable(typeof(V2StreamMetadata))]
	[JsonSerializable(typeof(V2Report))]
	[JsonSerializable(typeof(V2Heartbeat))]
	[JsonSerializable(typeof(V2PingRequest))]
	[JsonSerializable(typeof(V2Pong))]
	[JsonSerializable(typeof(V2QueryLatestRequest))]
	[JsonSerializable(typeof(V2Collection))]
	[JsonSerializable(typeof(V2SocketError))]
	[JsonSourceGenerationOptions(Converters = [typeof(NonstandardDateTimeJsonConverter), typeof(PositionConverter)])]
	sealed partial class V2SerializerContext : JsonSerializerContext { }
}
