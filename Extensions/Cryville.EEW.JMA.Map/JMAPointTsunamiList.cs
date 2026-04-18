using Cryville.EEW.JMA.Map.Resources;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.JMA.Map {
	public class JMAPointTsunamiList {
		static JMAPointTsunamiList? s_instance;
		public static JMAPointTsunamiList Instance => s_instance ??= new();

		public IReadOnlyDictionary<string, JMATsunamiPoint> Points { get; private set; }
		JMAPointTsunamiList() {
			Points = JsonSerializer.Deserialize(JMAPointTsunami.Data, SerializerContext.Default.IReadOnlyDictionaryStringJMATsunamiPoint)
				?? throw new InvalidOperationException("Invalid PointTsunami data.");
		}
	}

	public record JMATsunamiPoint(
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("lat")] float Latitude,
		[property: JsonPropertyName("lon")] float Longitude,
		[property: JsonPropertyName("affi")] string Affiliation,
		[property: JsonPropertyName("area")] string Area
	);
}
