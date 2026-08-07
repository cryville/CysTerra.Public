using Cryville.EEW.GeoJSON;
using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.USGS.Model {
	public record USGSEarthquakes(
		[property: JsonPropertyName("metadata")] USGSGeoJSONMetadata Metadata,
		Feature<USGSEarthquakeSummary>[] Features,
		double[]? BoundingBox
	) : FeatureCollection<USGSEarthquakeSummary>(Features, BoundingBox);

	public record USGSGeoJSONMetadata(
		[property: JsonPropertyName("generated")] long GeneratedTimestamp,
		[property: JsonPropertyName("uri")] Uri Uri,
		[property: JsonPropertyName("title")] string Title,
		[property: JsonPropertyName("status")] int Status,
		[property: JsonPropertyName("api")] string API,
		[property: JsonPropertyName("count")] int Count
	);
}
