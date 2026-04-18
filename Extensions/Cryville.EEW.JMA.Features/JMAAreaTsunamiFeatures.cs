using Cryville.EEW.GeoJSON;
using Cryville.EEW.GeoJSON.Features;
using Cryville.EEW.JMA.Map;
using Cryville.EEW.JMA.Map.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using static Cryville.EEW.JMA.Features.ExtraTagTypeKeys;
using static Cryville.EEW.TagTypeKeys;
using Feature = Cryville.EEW.Features.Feature;

namespace Cryville.EEW.JMA.Features {
	public class JMAAreaTsunamiFeatures {
		static JMAAreaTsunamiFeatures? s_instance;
		public static JMAAreaTsunamiFeatures Instance => s_instance ??= new();

		public IReadOnlyDictionary<string, Feature> Areas { get; private set; }
		public Feature Outline { get; private set; }
		JMAAreaTsunamiFeatures() {
			var data = JsonSerializer.Deserialize(JMAAreaTsunami.GISData, SerializerContext.Default.FeatureCollectionJMAAreaProperties)
				?? throw new InvalidOperationException("Invalid AreaTsunami data.");
			Areas = data.Features
				.Where(i => i.Properties.Code != "0")
				.ToDictionary(
					i => i.Properties.Code,
					f => new Feature(f.Geometry.ToFeaturesGeometry()) {
						{ Is, PlaceInfoArea },
						{ AreaLevel, 4 },
						{ Subject, Tsunami },
						{ Ref, f.Properties.Code },
						{ Name, (LocalizableCollection<string>)[
							new Localized<string>(f.Properties.Name, Local.Culture),
							new Localized<string>(f.Properties.NameKana, Local.CultureHrkt),
							JMAMessages.AreaTsunami().RootMessageStringSet.GetStringRequired(f.Properties.Code),
						] },
					}
				);
			var dataOutline = JsonSerializer.Deserialize(JMAAreaTsunami.GISDataOutline, GeoJSONSerializerContext.Default.MultiPolygon)
				?? throw new InvalidOperationException("Invalid AreaTsunamiOutline data.");
			Outline = new Feature(dataOutline.ToFeaturesGeometry()) {
				{ Is, PlaceInfoAreaMask },
				{ AreaLevel, 2 },
				{ Subject, Tsunami },
			};
		}
	}
}
