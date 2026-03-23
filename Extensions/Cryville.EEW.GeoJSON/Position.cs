using Cryville.Common.Compat;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.GeoJSON {
	/// <summary>
	/// The fundamental geometry construct.
	/// </summary>
	/// <param name="Longitude">The longitude or easting.</param>
	/// <param name="Latitude">The latitude or northing.</param>
	/// <param name="Altitude">The altitude or elevation.</param>
	public record struct Position(double Longitude, double Latitude, double? Altitude = null);

	/// <summary>
	/// Converts instances of the <see cref="Position" /> struct to or from JSON.
	/// </summary>
	public class PositionConverter : JsonConverter<Position> {
		/// <inheritdoc />
		public override Position Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			Debug.Assert(typeToConvert == typeof(Position));
			if (reader.TokenType != JsonTokenType.StartArray)
				throw new JsonException(null, new FormatException("Invalid Position."));
			reader.Read();
			var lon = reader.GetDouble();
			reader.Read();
			var lat = reader.GetDouble();
			reader.Read();
			if (reader.TokenType == JsonTokenType.EndArray) return new(lon, lat);
			var alt = reader.GetDouble();
			reader.Read();
			if (reader.TokenType != JsonTokenType.EndArray)
				throw new JsonException(null, new FormatException("Invalid Position."));
			return new(lon, lat, alt);
		}

		/// <inheritdoc />
		public override void Write(Utf8JsonWriter writer, Position value, JsonSerializerOptions options) {
			ThrowHelper.ThrowIfNull(writer);
			writer.WriteStartArray();
			writer.WriteNumberValue(value.Longitude);
			writer.WriteNumberValue(value.Latitude);
			if (value.Altitude != null) writer.WriteNumberValue(value.Altitude.Value);
			writer.WriteEndArray();
		}
	}
}
