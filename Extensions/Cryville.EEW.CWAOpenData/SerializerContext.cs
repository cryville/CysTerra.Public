using Cryville.EEW.CWAOpenData.Model;
using System.Text.Json.Serialization;

namespace Cryville.EEW.CWAOpenData {
	[JsonSerializable(typeof(CWAResult))]
	[JsonSourceGenerationOptions(Converters = [typeof(NonstandardDateTimeJsonConverter)])]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
