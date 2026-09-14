using Cryville.EEW.GeoJSON;
using System.Text.Json.Serialization;

namespace Cryville.EEW.CWAOpenData.Features {
	[JsonSerializable(typeof(FeatureCollection<CWATsunamiAreaProperties>))]
	[JsonSourceGenerationOptions(Converters = [typeof(PositionConverter)])]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
