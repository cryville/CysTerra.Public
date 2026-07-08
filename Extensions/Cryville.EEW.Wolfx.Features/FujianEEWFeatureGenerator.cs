using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.Wolfx.Model;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.Wolfx.Features {
	public class FujianEEWFeatureGenerator : IGenerator<FujianEEW, Feature?> {
		public Feature? Generate(FujianEEW e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			var f = new Feature {
				{ Is, ReportForecast },
				{ Subject, new Feature {
					{ Is, Earthquake },
					{ Ongoing, true },
					{ Time, new DateTimeOffset(e.OriginTime, Local.TimeZoneOffset) },
					{ At, new Feature(new Point(e.Longitude, e.Latitude)) {
						{ Is, Hypocenter },
						{ Name, new Localized<string>(e.HypoCenter, Local.Culture) },
					} },
					{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
				} },
				{ TimeModified, new DateTimeOffset(e.ReportTime, Local.TimeZoneOffset) },
			};
			return f;
		}
	}
}
