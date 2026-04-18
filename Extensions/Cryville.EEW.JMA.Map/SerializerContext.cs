using Cryville.EEW.GeoJSON;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cryville.EEW.JMA.Map {
	[JsonSerializable(typeof(IReadOnlyCollection<JMASeisStation>))]
	[JsonSerializable(typeof(IReadOnlyDictionary<string, JMASeisArea>))]
	[JsonSerializable(typeof(FeatureCollection<JMAAreaProperties>))]
	[JsonSerializable(typeof(IReadOnlyDictionary<string, JMATsunamiPoint>))]
	[JsonSourceGenerationOptions(Converters = [typeof(PositionConverter)])]
	public sealed partial class SerializerContext : JsonSerializerContext { }
}
