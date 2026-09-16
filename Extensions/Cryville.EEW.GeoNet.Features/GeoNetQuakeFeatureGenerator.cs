using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.GeoJSON.Features;
using Cryville.EEW.GeoNet.Model;
using Cryville.Measure;
using System.Collections.Generic;
using System.Globalization;
using static Cryville.EEW.GeoNet.Features.ExtraTagTypeKeys;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.GeoNet.Features {
	public class GeoNetQuakeFeatureGenerator : IGenerator<GeoJSON.Feature<GeoNetQuake>, Feature> {
		public Feature Generate(GeoJSON.Feature<GeoNetQuake> e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			return Generate(e.Geometry, e.Properties);
		}

		internal static Feature Generate(GeoJSON.Geometry geometry, GeoNetQuake properties) {
			var f = new Feature {
				{ Is, TagTypeKeys.Report },
				{ Subject, new Feature {
					{ Is, Earthquake },
					{ At, new Feature(geometry.ToFeaturesGeometry()) {
						{ Is, Hypocenter },
						{ Name, new Localized<string>(properties.Locality, Local.Culture) },
						{ HypocenterDepth, new Quantity(properties.Depth, DerivedMeasures.Kilometre) },
					} },
					{ Time, properties.Time },
					{ Magnitude, new Quantity(properties.Magnitude, Units.Dimensionless) },
				} },
			};
			if (properties.MMI > 0)
				f.Add(Includes, (List<Feature>)[
					new Feature {
						{ Is, ReportEstimation },
						{ IntensityMMI, properties.MMI },
					}
				]);
			if (properties is GeoNetQuakeHistory history)
				f.Add(TimeModified, history.ModificationTime);
			return f;
		}
	}
	public class GeoNetQuakeHistoryFeatureGenerator : IGenerator<GeoJSON.Feature<GeoNetQuakeHistory>, Feature> {
		public Feature Generate(GeoJSON.Feature<GeoNetQuakeHistory> e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			return GeoNetQuakeFeatureGenerator.Generate(e.Geometry, e.Properties);
		}
	}
}
