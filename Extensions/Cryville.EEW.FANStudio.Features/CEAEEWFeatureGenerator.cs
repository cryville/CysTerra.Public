using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.FANStudio.Features {
	public class CEAEEWFeatureGenerator : IGenerator<CEAEEW, Feature> {
		public Feature Generate(CEAEEW e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, ReportForecast },
				{ Subject, GenerateFromEarthquake(e) },
			};
			if (e.EpicenterIntensity is float epiIntensity)
				f.Add(IntensityCSIS, new QuantityInc(epiIntensity, 0.05f, Units.Dimensionless));
			if (e.UpdateTime is DateTime updateTime)
				f.Add(TimeModified, new DateTimeOffset(updateTime, Local.TimeZoneOffset));
			return f;
		}
		static Feature GenerateFromEarthquake(CEAEEW e) {
			var f = new Feature {
				{ Is, Earthquake },
				{ Ongoing, true },
				{ Time, new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) },
				{ At, GenerateFromLocation(e) },
				{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
			};
			return f;
		}
		static Feature GenerateFromLocation(CEAEEW e) {
			var f = new Feature(new Point(e.Longitude, e.Latitude)) {
				{ Is, Hypocenter },
				{ Name, new Localized<string>(e.PlaceName, Local.Culture) },
			};
			if (e.Depth is float depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5f, DerivedMeasures.Kilometre));
			return f;
		}
	}
}
