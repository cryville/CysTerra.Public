using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.FANStudio.Features {
	public class ShakeAlertEEWFeatureGenerator : IGenerator<ShakeAlertEEW, Feature> {
		public Feature Generate(ShakeAlertEEW e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, ReportForecast },
				{ Subject, GenerateFromEarthquake(e) },
			};
			return f;
		}
		static Feature GenerateFromEarthquake(ShakeAlertEEW e) {
			var f = new Feature {
				{ Is, Earthquake },
				{ Ongoing, true },
				{ Time, new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) },
				{ At, GenerateFromLocation(e) },
			};
			if (e.Magnitude is float mag)
				f.Add(Magnitude, new QuantityInc(mag, 0.005f, Units.Dimensionless));
			return f;
		}
		static Feature GenerateFromLocation(ShakeAlertEEW e) {
			var f = new Feature(new Point(e.Longitude, e.Latitude)) {
				{ Is, Hypocenter },
				{ Name, new Localized<string>(e.PlaceName, Local.UnitedStatesCulture) },
			};
			if (e.Depth is float depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5f, DerivedMeasures.Kilometre));
			return f;
		}
	}
}
