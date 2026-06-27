using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.FANStudio.Features {
	public class BeijingEarthquakeFeatureGenerator : IGenerator<BeijingEarthquake, Feature> {
		public Feature Generate(BeijingEarthquake e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			return new Feature {
				{ Is, Earthquake },
				{ Time, new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) },
				{ At, new Feature(new Point(e.Longitude, e.Latitude)) {
					{ Is, Hypocenter },
					{ Name, new Localized<string>(e.PlaceName, Local.Culture) },
					{ HypocenterDepth, new QuantityInc(e.Depth, 0.5f, DerivedMeasures.Kilometre) },
				} },
				{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
			};
		}
	}
}
