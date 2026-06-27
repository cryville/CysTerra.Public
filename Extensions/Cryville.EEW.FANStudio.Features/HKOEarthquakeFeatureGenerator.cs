using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.FANStudio.Features {
	public class HKOEarthquakeFeatureGenerator : IGenerator<HKOEarthquake, Feature> {
		public Feature Generate(HKOEarthquake e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			return new Feature {
				{ Is, Earthquake },
				{ Time, new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) },
				{ At, new Feature(new Point(e.Longitude, e.Latitude)) {
					{ Is, Hypocenter },
					{ Name, new Localized<string>(e.Region, Local.HongKongCulture) },
					{ HypocenterDepth, new QuantityInc(e.Depth, 0.5f, DerivedMeasures.Kilometre) },
				} },
				{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
				// TODO EvaluationMode
			};
		}
	}
}
