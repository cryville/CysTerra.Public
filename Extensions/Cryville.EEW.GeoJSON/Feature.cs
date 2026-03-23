using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.GeoJSON {
	/// <summary>
	/// Represents a spatially bounded thing.
	/// </summary>
	/// <param name="Id">A JSON string or number representing the commonly used identifier of the feature.</param>
	/// <param name="Geometry">The geometry of the feature.</param>
	/// <param name="Properties">The properties of the feature.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	public record Feature(
		[property: JsonPropertyName("id")] JsonElement Id,
		[property: JsonPropertyName("geometry")] Geometry Geometry,
		[property: JsonPropertyName("properties")] IDictionary<string, JsonElement>? Properties,
		double[]? BoundingBox
	) : GeoJSONObject(BoundingBox);

	/// <summary>
	/// Represents a spatially bounded thing.
	/// </summary>
	/// <typeparam name="T">The type of the properties.</typeparam>
	/// <param name="Id">A JSON string or number representing the commonly used identifier of the feature.</param>
	/// <param name="Geometry">The geometry of the feature.</param>
	/// <param name="Properties">The properties of the feature.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	public record Feature<T>(
		[property: JsonPropertyName("id")] JsonElement Id,
		[property: JsonPropertyName("geometry")] Geometry Geometry,
		[property: JsonPropertyName("properties")] T Properties,
		double[]? BoundingBox
	) : GeoJSONObject(BoundingBox), IFeature<T>;

	/// <summary>
	/// Represents a spatially bounded thing.
	/// </summary>
	/// <typeparam name="T">The type of the properties.</typeparam>
	public interface IFeature<out T> {
		/// <summary>
		/// A JSON string or number representing the commonly used identifier of the feature.
		/// </summary>
		JsonElement Id { get; }
		/// <summary>
		/// The geometry of the feature.
		/// </summary>
		Geometry Geometry { get; }
		/// <summary>
		/// The properties of the feature.
		/// </summary>
		T Properties { get; }
		/// <summary>
		/// The bounding box.
		/// </summary>
		double[]? BoundingBox { get; }
	}
}
