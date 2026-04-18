using System.Text.Json.Serialization;

namespace Cryville.EEW.JMA.Map {
	public sealed record JMAAreaProperties(
		[property: JsonPropertyName("code")] string Code,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("namekana")] string NameKana
	);
}
