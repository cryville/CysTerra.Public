using Cryville.EEW.CWAOpenData.Features.Resources;
using Cryville.EEW.GeoJSON;
using Cryville.EEW.GeoJSON.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Cryville.EEW.TagTypeKeys;
using Feature = Cryville.EEW.Features.Feature;

namespace Cryville.EEW.CWAOpenData.Features {
	public class CWATsunamiAreaFeatures {
		static CWATsunamiAreaFeatures? s_instance;
		public static CWATsunamiAreaFeatures Instance => s_instance ??= new();

		static readonly TagTypeKey TagPlaceInfoAreaMask = Place.OfSubtype("info_area_mask");

		public IReadOnlyDictionary<string, Feature> Areas { get; private set; }
		public Feature Outline { get; private set; }
		CWATsunamiAreaFeatures() {
			var data = JsonSerializer.Deserialize(CWATsunamiAreas.GISData, SerializerContext.Default.FeatureCollectionCWATsunamiAreaProperties)
				?? throw new InvalidOperationException("Invalid CWATsunamiAreas data.");
			Areas = data.Features
				.Where(i => !string.IsNullOrWhiteSpace(i.Properties.Name))
				.ToDictionary(i => i.Properties.Name, i => new Feature(i.Geometry.ToFeaturesGeometry()) {
					{ Is, PlaceInfoArea },
					{ AreaLevel, 3 },
					{ Subject, Tsunami },
					{ Name, (LocalizableCollection<string>)[
						new Localized<string>(i.Properties.Name, Local.Culture),
						SharedResources.TsunamiForecastArea().GetStringRequired(i.Properties.Name),
					] },
				});
			var dataOutline = JsonSerializer.Deserialize(CWATsunamiAreas.GISDataOutline, GeoJSONSerializerContext.Default.MultiPolygon)
				?? throw new InvalidOperationException("Invalid CWATsunamiAreasOutline data.");
			Outline = new Feature(dataOutline.ToFeaturesGeometry()) {
				{ Is, TagPlaceInfoAreaMask },
				{ AreaLevel, 2 },
				{ Subject, Tsunami },
			};
		}
	}

	sealed record CWATsunamiAreaProperties(
		[property: JsonPropertyName("name")] string Name
	);
}
