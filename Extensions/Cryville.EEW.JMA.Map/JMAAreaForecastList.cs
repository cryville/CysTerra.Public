using Cryville.EEW.GeoJSON;
using Cryville.EEW.JMA.Map.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Cryville.EEW.JMA.Map {
	public class JMAAreaForecastList {
		static JMAAreaForecastList? s_instance;
		public static JMAAreaForecastList Instance => s_instance ??= new();

		public IReadOnlyDictionary<string, Geometry> Areas { get; private set; }
		JMAAreaForecastList() {
			var data = JsonSerializer.Deserialize(JMAAreaForecastLocalE.GISData, SerializerContext.Default.FeatureCollectionJMAAreaProperties)
				?? throw new InvalidOperationException("Invalid JMAAreaForecastEEW data.");
			Areas = data.Features
				.Where(i => !string.IsNullOrEmpty(i.Properties.Code))
				.ToDictionary(i => i.Properties.Code, i => i.Geometry);
		}
	}
}
