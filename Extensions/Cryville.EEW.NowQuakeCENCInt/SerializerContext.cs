using Cryville.EEW.NowQuakeCENCInt.Model;
using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.NowQuakeCENCInt {
	[Obsolete("Use the V2 protocol instead.")]
	[JsonSerializable(typeof(WsMessage))]
	[JsonSourceGenerationOptions(Converters = [typeof(NonstandardDateTimeJsonConverter)])]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
