using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Cryville.EEW.GeoJSON {
	/// <summary>
	/// Represents a feature collection.
	/// </summary>
	/// <param name="Features">The features.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
	[SuppressMessage("Naming", "CA1711", Justification = "[sic]")]
	public record FeatureCollection(
		[property: JsonPropertyName("features")] Feature[] Features,
		double[]? BoundingBox
	) : GeoJSONObject(BoundingBox);

	/// <summary>
	/// Represents a feature collection.
	/// </summary>
	/// <typeparam name="T">The type of the properties of the features.</typeparam>
	/// <param name="Features">The features.</param>
	/// <param name="BoundingBox">The bounding box.</param>
	[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
	[SuppressMessage("Naming", "CA1711", Justification = "[sic]")]
	public record FeatureCollection<T>(
		[property: JsonPropertyName("features")] Feature<T>[] Features,
		double[]? BoundingBox
	) : GeoJSONObject(BoundingBox);
}
