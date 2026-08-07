using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.USGS.Model {
	public record USGSEarthquakeProduct(
		[property: JsonPropertyName("indexid")] object IndexID,
		[property: JsonPropertyName("indexTime")] long IndexTimestamp,
		[property: JsonPropertyName("id")] string ID,
		[property: JsonPropertyName("type")] string Type,
		[property: JsonPropertyName("code")] string Code,
		[property: JsonPropertyName("source")] string Source,
		[property: JsonPropertyName("updateTime")] long UpdateTimestamp,
		[property: JsonPropertyName("status")] string Status,
		[property: JsonPropertyName("properties")] IReadOnlyDictionary<string, string> Properties,
		[property: JsonPropertyName("preferredWeight")] int PreferredWeight,
		[property: JsonPropertyName("contents"), JsonConverter(typeof(USGSProductContentsConverter))] Dictionary<string, USGSProductContent>? Contents
	);

	sealed class USGSProductContentsConverter : JsonConverter<Dictionary<string, USGSProductContent>?> {
		public override Dictionary<string, USGSProductContent>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			if (reader.TokenType == JsonTokenType.StartArray) {
				reader.Read();
				if (reader.TokenType != JsonTokenType.EndArray)
					throw new JsonException(null, new FormatException("Expect object or empty array. Got non-empty array."));
				return null;
			}
			return (Dictionary<string, USGSProductContent>?)JsonSerializer.Deserialize(ref reader, options.GetTypeInfo(typeToConvert));
		}
		public override void Write(Utf8JsonWriter writer, Dictionary<string, USGSProductContent>? value, JsonSerializerOptions options) {
			if (value == null) {
				writer.WriteStartArray();
				writer.WriteEndArray();
				return;
			}
			JsonSerializer.Serialize(writer, value, options.GetTypeInfo(value.GetType()));
		}
	}

	public record USGSProductContent(
		[property: JsonPropertyName("contentType")] string ContentType,
		[property: JsonPropertyName("lastModified")] long LastModified,
		[property: JsonPropertyName("length")] int Length,
		[property: JsonPropertyName("url")] Uri? Url,
		[property: JsonPropertyName("bytes")] string? Body
	);
}
