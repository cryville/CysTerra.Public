using System.Text.Json.Serialization;

namespace Cryville.EEW.BMKGOpenData {
	[JsonSerializable(typeof(BMKGEarthquakeLatest))]
	[JsonSerializable(typeof(BMKGEarthquakeList))]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
