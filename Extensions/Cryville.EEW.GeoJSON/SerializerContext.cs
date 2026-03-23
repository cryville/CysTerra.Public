using System.Text.Json.Serialization;

namespace Cryville.EEW.GeoJSON {
	/// <summary>
	/// <see cref="JsonSerializerContext" /> for GeoJSON objects.
	/// </summary>
	[JsonSerializable(typeof(GeoJSONObject))]
	[JsonSourceGenerationOptions(Converters = [typeof(PositionConverter)])]
	public sealed partial class GeoJSONSerializerContext : JsonSerializerContext { }
}
