using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.Report;
using Cryville.Measure;
using System.Collections.Generic;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;
using static Cryville.EEW.BMKGOpenData.Features.ExtraTagTypeKeys;

namespace Cryville.EEW.BMKGOpenData.Features {
	public class BMKGEarthquakeFeatureGenerator : IGenerator<BMKGEarthquake, Feature> {
		public Feature Generate(BMKGEarthquake e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, TagTypeKeys.Report },
				{ Subject, GenerateFromEarthquake(e) },
			};
			if (e.Intensity is string intensity) {
				var fIncludes = new List<Feature>();
				f.Add(Includes, new Feature {
					{ Is, ReportEstimation },
					{ Includes, fIncludes },
				});
				foreach (var (_, maxIntensity, region) in BMKGHelpers.ExtractIntensity(intensity)) {
					// TODO minIntensity
					int? maxIntensityValue = RomanNumerals.RomanToInteger(maxIntensity);
					if (maxIntensityValue == 0)
						maxIntensityValue = null;
					fIncludes.Add(new Feature {
						{ Is, ReportEstimation },
						{ At, new Feature(UnknownGeometry.Instance) {
							{ Is, PlaceInfoArea },
							{ Name, new Localized<string>(region, Local.Culture) },
						} },
						{ IntensityMMI, maxIntensityValue },
					});
				}
			}
			return f;
		}

		static Feature GenerateFromEarthquake(BMKGEarthquake e) {
			var f = new Feature {
				{ Is, Earthquake },
				{ At, GenerateFromHypocenter(e) },
				{ Time, e.DateTime },
			};
			if (float.TryParse(e.Magnitude, NumberStyles.Float, CultureInfo.InvariantCulture, out float mag))
				f.Add(Magnitude, new QuantityInc(mag, 0.05f, Units.Dimensionless));
			return f;
		}

		static Feature GenerateFromHypocenter(BMKGEarthquake e) {
			Geometry point = UnknownGeometry.Instance;
			if (BMKGHelpers.ExtractCoordinates(e.Coordinates, out float lat, out float lon))
				point = new Point(lon, lat);
			var f = new Feature(point) {
				{ Is, Hypocenter },
				{ Name, new Localized<string>(e.Region, Local.Culture) },
			};
			if (BMKGHelpers.ExtractDepth(e.Depth) is float depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5f, DerivedMeasures.Kilometre));
			return f;
		}
	}
}
