using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.FANStudio.Features {
	public class CWAEEWFeatureGenerator : IGenerator<CWAEEW, Feature> {
		public Feature Generate(CWAEEW e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, ReportForecast },
				{ Subject, GenerateFromEarthquake(e) },
			};
			// TODO e.LocationDesc
			return f;
		}
		static Feature GenerateFromEarthquake(CWAEEW e) {
			var f = new Feature {
				{ Is, Earthquake },
				{ Ongoing, true },
				{ Time, new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) },
				{ At, GenerateFromLocation(e) },
				{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
			};
			return f;
		}
		static Feature GenerateFromLocation(CWAEEW e) {
			var f = new Feature(new Point(e.Longitude, e.Latitude)) {
				{ Is, Hypocenter },
				{ Name, new Localized<string>(e.PlaceName, Local.TaiwanCulture) },
			};
			if (e.Depth is float depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 5f, DerivedMeasures.Kilometre));
			return f;
		}
	}
}
