using Cryville.EEW.GeoJSON;
using Cryville.EEW.GeoNet.Model;
using System.Text.Json.Serialization;

namespace Cryville.EEW.GeoNet {
	[JsonSerializable(typeof(FeatureCollection<GeoNetQuake>))]
	[JsonSerializable(typeof(FeatureCollection<GeoNetQuakeHistory>))]
	[JsonSerializable(typeof(GeoNetStrong))]
	[JsonSourceGenerationOptions(Converters = [typeof(PositionConverter)])]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
