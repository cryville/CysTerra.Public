using Cryville.EEW.GeoJSON;
using System.Text.Json.Serialization;

namespace Cryville.EEW.EMSC {
	[JsonSerializable(typeof(EMSCRealTimeAction))]
	[JsonSerializable(typeof(Feature<EMSCRealTimeEvent>))]
	[JsonSourceGenerationOptions(Converters = [typeof(PositionConverter)])]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
