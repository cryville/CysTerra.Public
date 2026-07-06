using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.FANStudio.Features {
	public class YunnanEarthquakeFeatureGenerator : IGenerator<YunnanEarthquake, Feature?> {
		public Feature? Generate(YunnanEarthquake e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, Earthquake },
				{ Time, new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) },
				{ At, GenerateFromLocation(e) },
			};
			if (e.Magnitude is float magnitude)
				f.Add(Magnitude, new QuantityInc(magnitude, 0.05f, Units.Dimensionless));
			if (e.MagnitudeL is float magnitudeL)
				f.Add(MagnitudeRichter, new QuantityInc(magnitudeL, 0.05f, Units.Dimensionless));
			return f;
		}
		static Feature GenerateFromLocation(YunnanEarthquake e) {
			Point? point = null;
			if (e.Latitude is float latitude && e.Longitude is float longitude)
				point = new Point(longitude, latitude);
			var f = new Feature(point) {
				{ Is, Hypocenter },
			};
			if (e.PlaceName is string placeName)
				f.Add(Name, new Localized<string>(placeName, Local.Culture));
			if (e.Depth is float depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5f, DerivedMeasures.Kilometre));
			return f;
		}
	}
}
