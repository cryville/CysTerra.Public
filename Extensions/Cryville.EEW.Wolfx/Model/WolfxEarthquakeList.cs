using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Cryville.EEW.Wolfx.Model {
	public record WolfxEarthquakeList<T>([property: JsonPropertyName("md5")] string MD5) : BaseModel {
		static readonly JsonTypeInfo<T> _jsonTypeInfo = (JsonTypeInfo<T>)(SerializerContext.Default.GetTypeInfo(typeof(T)) ?? throw new InvalidCastException($"Type info for {typeof(T)} not found."));

		[JsonExtensionData]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		[SuppressMessage("Usage", "CA2227", Justification = "DTO")]
		public Dictionary<string, JsonElement> ExtensionData { get; set; } = [];

		ICollection<T>? m_earthquakes;
		public ICollection<T> Earthquakes => m_earthquakes ??= [.. EnumerateUnordered()
			.OrderBy(e => e.Item1)
			.Select(e => e.Item2)
		];
		IEnumerable<(int, T)> EnumerateUnordered() {
			foreach (var e in ExtensionData) {
				string key = e.Key;
				if (!key.StartsWith("No", StringComparison.OrdinalIgnoreCase)) continue;
				if (!int.TryParse(key[2..], NumberStyles.Integer, CultureInfo.InvariantCulture, out int num)) continue;
				yield return (num, e.Value.Deserialize(_jsonTypeInfo) ?? throw new JsonException("Null event."));
			}
		}
		public override string ToString() => string.Join('\n', Earthquakes);
	}
}
