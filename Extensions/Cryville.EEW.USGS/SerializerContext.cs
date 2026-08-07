using Cryville.EEW.GeoJSON;
using Cryville.EEW.USGS.Model;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.USGS {
	[JsonSerializable(typeof(USGSEarthquakes))]
	[JsonSerializable(typeof(Feature<USGSEarthquakeDetail>))]
	[JsonSerializable(typeof(USGSContours))]
	[JsonSourceGenerationOptions(Converters = [typeof(PositionConverter), typeof(MalformedDoubleArrayJsonConverter)])]
	sealed partial class SerializerContext : JsonSerializerContext { }

	[SuppressMessage("Performance", "CA1812", Justification = "Used in serialization")]
	sealed partial class MalformedDoubleArrayJsonConverter : JsonConverter<double[]> {
		public override double[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			if (reader.TokenType == JsonTokenType.String)
				return JsonSerializer.Deserialize(reader.GetString()!, DoubleArraySerializerContext.Default.DoubleArray);
			return JsonSerializer.Deserialize(ref reader, DoubleArraySerializerContext.Default.DoubleArray);
		}
		public override void Write(Utf8JsonWriter writer, double[] value, JsonSerializerOptions options) {
			JsonSerializer.Serialize(writer, value, DoubleArraySerializerContext.Default.DoubleArray);
		}

		[JsonSerializable(typeof(double[]))]
		sealed partial class DoubleArraySerializerContext : JsonSerializerContext { }
	}
}
