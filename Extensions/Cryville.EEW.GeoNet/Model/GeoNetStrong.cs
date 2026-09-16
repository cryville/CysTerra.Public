using Cryville.EEW.GeoJSON;
using System.Text.Json.Serialization;

namespace Cryville.EEW.GeoNet.Model {
	public record GeoNetStrong(
		[property: JsonPropertyName("metadata")] GeoNetStrongMetadata Metadata,
		Feature<GeoNetStrongStation>[] Features,
		double[]? BoundingBox
	) : FeatureCollection<GeoNetStrongStation>(Features, BoundingBox);
	public record GeoNetStrongMetadata(
		[property: JsonPropertyName("author")] string Author,
		[property: JsonPropertyName("depth")] float Depth,
		[property: JsonPropertyName("description")] string Description,
		[property: JsonPropertyName("event_id")] string EventID,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("mag_type")] string MagnitudeType,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("version")] string Version
	);
	public record GeoNetStrongStation(
		[property: JsonPropertyName("distance")] float Distance,
		[property: JsonPropertyName("location")] string Location,
		[property: JsonPropertyName("mmi")] int MMI,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("network")] string Network,
		[property: JsonPropertyName("pga_h")] double PGAHorizontal,
		[property: JsonPropertyName("pga_v")] double PGAVertical,
		[property: JsonPropertyName("pgv_h")] double PGVHorizontal,
		[property: JsonPropertyName("pgv_v")] double PGVVertical,
		[property: JsonPropertyName("station")] string Station
	);
}
