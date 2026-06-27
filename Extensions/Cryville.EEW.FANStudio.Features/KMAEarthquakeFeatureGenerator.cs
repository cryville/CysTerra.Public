using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.FANStudio.Features {
	public class KMAEarthquakeFeatureGenerator : IGenerator<KMAEarthquake, Feature> {
		public Feature Generate(KMAEarthquake e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, ReportObservation },
				{ Subject, new Feature {
					{ Is, Earthquake },
					{ Time, new DateTimeOffset(e.ShockTime, Local.SouthKoreaTimeZoneOffset) },
					{ At, GenerateFromHypocenter(e) },
					{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
					{ TimeModified, new DateTimeOffset(e.CreateTime, Local.SouthKoreaTimeZoneOffset) },
				} },
			};
			if (e.EpicenterIntensity is int epiIntensity)
				f.Add(IntensityMMI, epiIntensity);
			return f;
		}
		static Feature GenerateFromHypocenter(KMAEarthquake e) {
			var f = new Feature(new Point(e.Longitude, e.Latitude)) {
				{ Is, Hypocenter },
				{ Name, new Localized<string>(e.PlaceName, Local.SouthKoreaCulture) },
			};
			if (e.Depth is float depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5f, DerivedMeasures.Kilometre));
			return f;
		}
	}
}
