using Cryville.EEW.Features;
using Cryville.EEW.GeoJSON.Features;
using Cryville.EEW.JMA.Map;
using Cryville.EEW.JMA.Map.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.JMA.Features {
	public class JMAAreaForecastLocalEarthquakeFeatures {
		static JMAAreaForecastLocalEarthquakeFeatures? s_instance;
		public static JMAAreaForecastLocalEarthquakeFeatures Instance => s_instance ??= new();

		public IReadOnlyDictionary<string, Feature> Areas { get; private set; }
		JMAAreaForecastLocalEarthquakeFeatures() {
			var data = JsonSerializer.Deserialize(JMAAreaForecastLocalE.GISData, SerializerContext.Default.FeatureCollectionJMAAreaProperties)
				?? throw new InvalidOperationException("Invalid JMAAreaForecastLocalE data.");
			Areas = data.Features
				.Where(f => !string.IsNullOrEmpty(f.Properties.Code))
				.ToDictionary(
					f => f.Properties.Code,
					f => new Feature(f.Geometry.ToFeaturesGeometry()) {
						{ Is, PlaceInfoArea },
						{ AreaLevel, 5 },
						{ Subject, Earthquake },
						{ Ref, f.Properties.Code },
						{ Name, (LocalizableCollection<string>)[
							new Localized<string>(f.Properties.Name, Local.Culture),
							new Localized<string>(f.Properties.NameKana, Local.CultureHrkt),
							JMAMessages.AreaForecastEEW().RootMessageStringSet.GetStringRequired(f.Properties.Code),
						] },
					}
				);
		}
	}
}
