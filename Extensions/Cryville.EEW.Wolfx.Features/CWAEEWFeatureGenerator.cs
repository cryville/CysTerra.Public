using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.Wolfx.Model;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.Wolfx.Features {
	public class CWAEEWFeatureGenerator : IGenerator<CWAEEW, Feature?> {
		public Feature? Generate(CWAEEW e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			if (e.IsCancellation)
				return null;
			var f = new Feature {
				{ Is, ReportForecast },
				{ Subject, new Feature {
					{ Is, Earthquake },
					{ Ongoing, true },
					{ Time, new DateTimeOffset(e.OriginTime, Local.TaiwanTimeZoneOffset) },
					{ At, new Feature(new Point(e.Longitude, e.Latitude)) {
						{ Is, Hypocenter },
						{ Name, new Localized<string>(e.HypoCenter, Local.TaiwanCulture) },
						{ HypocenterDepth, new QuantityInc(e.Depth, 5f, DerivedMeasures.Kilometre) },
					} },
					{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
				} },
				{ IntensityCWASIS, e.MaxIntensity },
				{ TimeModified, new DateTimeOffset(e.ReportTime, Local.TaiwanTimeZoneOffset) },
			};
			return f;
		}
	}
}
