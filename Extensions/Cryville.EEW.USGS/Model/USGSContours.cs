using Cryville.EEW.GeoJSON;
using System.Text.Json.Serialization;

namespace Cryville.EEW.USGS.Model {
	public record USGSContours(
		Feature<USGSContour>[] Features,
		double[]? BoundingBox,
		[property: JsonPropertyName("name")] string? Name,
		[property: JsonPropertyName("metadata")] USGSContoursMetadata Metadata
	) : FeatureCollection<USGSContour>(Features, BoundingBox);

	public record USGSContoursMetadata(
		[property: JsonPropertyName("eventid")] string EventID,
		[property: JsonPropertyName("longitude")] double Longitude,
		[property: JsonPropertyName("latitude")] double Latitude
	);

	public record USGSContour(
		[property: JsonPropertyName("value")] float Value,
		[property: JsonPropertyName("units")] string Units,
		[property: JsonPropertyName("color")] string Color,
		[property: JsonPropertyName("weight")] float Weight
	);
}
