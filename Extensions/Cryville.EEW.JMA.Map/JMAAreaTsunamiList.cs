using Cryville.EEW.GeoJSON;
using Cryville.EEW.JMA.Map.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Cryville.EEW.JMA.Map {
	public class JMAAreaTsunamiList {
		static JMAAreaTsunamiList? s_instance;
		public static JMAAreaTsunamiList Instance => s_instance ??= new();

		public IReadOnlyDictionary<string, Geometry> Areas { get; private set; }
		public MultiPolygon Outline { get; private set; }
		JMAAreaTsunamiList() {
			var data = JsonSerializer.Deserialize(JMAAreaTsunami.GISData, SerializerContext.Default.FeatureCollectionJMAAreaProperties)
				?? throw new InvalidOperationException("Invalid AreaTsunami data.");
			Areas = data.Features
				.Where(i => i.Properties.Code != "0")
				.ToDictionary(i => i.Properties.Code, i => i.Geometry);
			var dataOutline = JsonSerializer.Deserialize(JMAAreaTsunami.GISDataOutline, GeoJSONSerializerContext.Default.MultiPolygon)
				?? throw new InvalidOperationException("Invalid AreaTsunamiOutline data.");
			Outline = dataOutline;
		}
	}
}
