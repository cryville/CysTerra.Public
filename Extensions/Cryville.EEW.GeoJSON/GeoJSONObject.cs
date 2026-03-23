using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.GeoJSON {
	/// <summary>
	/// A Geometry, Feature, or collection of Features.
	/// </summary>
	/// <param name="BoundingBox">The bounding box.</param>
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
	[JsonDerivedType(typeof(Point), "Point")]
	[JsonDerivedType(typeof(MultiPoint), "MultiPoint")]
	[JsonDerivedType(typeof(LineString), "LineString")]
	[JsonDerivedType(typeof(MultiLineString), "MultiLineString")]
	[JsonDerivedType(typeof(Polygon), "Polygon")]
	[JsonDerivedType(typeof(MultiPolygon), "MultiPolygon")]
	[JsonDerivedType(typeof(GeometryCollection), "GeometryCollection")]
	[JsonDerivedType(typeof(Feature), "Feature")]
	[JsonDerivedType(typeof(FeatureCollection), "FeatureCollection")]
	public record GeoJSONObject(
		[property: JsonPropertyName("bbox")] double[]? BoundingBox
	) {
		/// <summary>
		/// Other members in the GeoJSON object.
		/// </summary>
		[JsonExtensionData]
		public IDictionary<string, JsonElement>? ExtensionData { get; set; }
	}
}
