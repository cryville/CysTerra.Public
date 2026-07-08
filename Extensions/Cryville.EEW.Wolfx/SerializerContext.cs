using Cryville.EEW.Wolfx.Model;
using System.Text.Json.Serialization;

namespace Cryville.EEW.Wolfx {
	[JsonSerializable(typeof(BaseModel))]
	[JsonSerializable(typeof(CENCEarthquake))]
	[JsonSerializable(typeof(JMAEarthquake))]
	[JsonSourceGenerationOptions(Converters = [typeof(NonstandardDateTimeJsonConverter)])]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
