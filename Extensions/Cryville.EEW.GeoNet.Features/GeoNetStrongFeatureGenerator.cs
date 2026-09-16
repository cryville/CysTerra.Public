using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.GeoJSON.Features;
using Cryville.EEW.GeoNet.Model;
using Cryville.Measure;
using System.Collections.Generic;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.GeoNet.Features {
	public class GeoNetStrongFeatureGenerator : IGenerator<GeoNetStrong, Feature> {
		public Feature Generate(GeoNetStrong e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var metadata = e.Metadata;
			var fIncludes = new List<Feature>();
			var f = new Feature {
				{ Is, ReportObservation },
				{ Subject, new Feature {
					{ Is, Earthquake },
					{ At, new Feature(new Point(metadata.Longitude, metadata.Latitude)) {
						{ Is, Hypocenter },
						{ Name, new Localized<string>(metadata.Description, Local.Culture) },
						{ HypocenterDepth, new QuantityInc(metadata.Depth, 0.05f, DerivedMeasures.Kilometre) },
					} },
					{ Magnitude, new QuantityInc(metadata.Magnitude, 0.05f, Units.Dimensionless) },
					{ Source, metadata.Author },
				} },
				{ Includes, fIncludes },
			};
			foreach (var feature in e.Features) {
				var p = feature.Properties;
				fIncludes.Add(new Feature {
					{ Is, ReportObservation },
					{ At, new Feature(feature.Geometry.ToFeaturesGeometry()) {
						{ Is, ManMadeMonitoringStation },
						{ MonitoringSeismicActivity, true },
						{ Name, new Localized<string>(p.Name, Local.Culture) },
						{ Ref, feature.Id },
					} },
					{ IntensityMMI, p.MMI },
					// TODO PGA, PGV
				});
			}
			return f;
		}
	}
}
