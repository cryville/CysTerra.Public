using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Cryville.EEW.GeoJSON {
	/// <summary>
	/// Points, curves, and surfaces in coordinate space.
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
	public abstract record Geometry(double[]? BoundingBox) : GeoJSONObject(BoundingBox);
	/// <summary>
	/// Represents a point.
	/// </summary>
	/// <param name="Coordinates">The coordinates, a single position.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	public record Point([property: JsonPropertyName("coordinates")] Position Coordinates, double[]? BoundingBox) : Geometry(BoundingBox);
	/// <summary>
	/// Represents multiple points.
	/// </summary>
	/// <param name="Coordinates">The coordinates, an array of positions.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	public record MultiPoint([property: JsonPropertyName("coordinates")] Position[] Coordinates, double[]? BoundingBox) : Geometry(BoundingBox);
	/// <summary>
	/// Represents a line string.
	/// </summary>
	/// <param name="Coordinates">The coordinates, an array of two or more positions.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	public record LineString([property: JsonPropertyName("coordinates")] Position[] Coordinates, double[]? BoundingBox) : Geometry(BoundingBox);
	/// <summary>
	/// Represents multiple line strings.
	/// </summary>
	/// <param name="Coordinates">The coordinates, an array of <see cref="LineString.Coordinates" /> arrays.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	public record MultiLineString([property: JsonPropertyName("coordinates")] Position[][] Coordinates, double[]? BoundingBox) : Geometry(BoundingBox);
	/// <summary>
	/// Represents a polygon.
	/// </summary>
	/// <param name="Coordinates">The coordinates, an array of linear ring coordinate arrays.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	public record Polygon([property: JsonPropertyName("coordinates")] Position[][] Coordinates, double[]? BoundingBox) : Geometry(BoundingBox);
	/// <summary>
	/// Represents multiple polygons.
	/// </summary>
	/// <param name="Coordinates">The coordinates, an array of <see cref="Polygon.Coordinates" /> arrays.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	public record MultiPolygon([property: JsonPropertyName("coordinates")] Position[][][] Coordinates, double[]? BoundingBox) : Geometry(BoundingBox);
	/// <summary>
	/// Represents a geometry collection.
	/// </summary>
	/// <param name="Geometries">An array of <see cref="Geometry" /> objects.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
	[SuppressMessage("Naming", "CA1711", Justification = "[sic]")]
	public record GeometryCollection([property: JsonPropertyName("geometries")] Geometry[] Geometries, double[]? BoundingBox) : Geometry(BoundingBox);
}
