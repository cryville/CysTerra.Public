using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.Wolfx.Model;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.Wolfx.Features {
	public class CENCEEWFeatureGenerator : IGenerator<CENCEEW, Feature> {
		public Feature Generate(CENCEEW e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, ReportForecast },
				{ Subject, GenerateFromEarthquake(e) },
				{ IntensityCSIS, new QuantityInc(e.MaxIntensity, 0.05f, Units.Dimensionless) },
				{ TimeModified, new DateTimeOffset(e.ReportTime, Local.TimeZoneOffset) },
			};
			return f;
		}
		static Feature GenerateFromEarthquake(CENCEEW e) {
			var f = new Feature {
				{ Is, Earthquake },
				{ Ongoing, true },
				{ Time, new DateTimeOffset(e.OriginTime, Local.TimeZoneOffset) },
				{ At, GenerateFromLocation(e) },
				{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
			};
			return f;
		}
		static Feature GenerateFromLocation(CENCEEW e) {
			var f = new Feature(new Point(e.Longitude, e.Latitude)) {
				{ Is, Hypocenter },
				{ Name, new Localized<string>(e.HypoCenter, Local.Culture) },
			};
			if (e.Depth is float depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5f, DerivedMeasures.Kilometre));
			return f;
		}
	}
}
