using System.Text.Json.Serialization;

namespace Cryville.EEW.MangxuProjectAPI.Model {
	public record MmdRaw(
		[property: JsonPropertyName("dist_malaysia")] string DistMalaysia,
		[property: JsonPropertyName("dist_nearest")] string DistNearest,
		[property: JsonPropertyName("location_original")] string LocationOriginal,
		[property: JsonPropertyName("mag_type")] string MagType
	);
}
