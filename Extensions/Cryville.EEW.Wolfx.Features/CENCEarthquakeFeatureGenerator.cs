using Cryville.Common.Compat;
using Cryville.EEW.ComponentModel;
using Cryville.EEW.Features;
using Cryville.EEW.Wolfx.Model;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.Wolfx.Features {
	public class CENCEarthquakeFeatureGenerator : IGenerator<CENCEarthquake, Feature>, IPropertiesHolder {
		[LocalizableDisplayName("PNUseRawLocationName")]
		[LocalizableDescription("PDUseRawLocationName")]
		public bool UseRawLocationName { get; set; }

		public Feature Generate(CENCEarthquake e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, Earthquake },
				{ Time, new DateTimeOffset(e.Time, Local.TimeZoneOffset) },
				{ At, GenerateFromLocation(e)  },
				// TODO EvaluationMode
			};
			if (float.TryParse(e.Magnitude, NumberStyles.Float, CultureInfo.InvariantCulture, out float magnitude))
				f.Add(Magnitude, new QuantityInc(magnitude, 0.05f, Units.Dimensionless));
			if (e.ReportTime is DateTime reportTime)
				f.Add(TimeModified, new DateTimeOffset(reportTime, Local.TimeZoneOffset));
			return f;
		}

		Feature GenerateFromLocation(CENCEarthquake e) {
			Point? point = null;
			if (
				float.TryParse(e.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
				float.TryParse(e.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon)
			) {
				point = new Point(lon, lat);
			}
			var f = new Feature(point) {
				{ Is, Hypocenter },
				{ Name, new Localized<string>(UseRawLocationName ? e.RawLocation : e.Location, Local.Culture) },
			};
			if (float.TryParse(e.Depth, NumberStyles.Float, CultureInfo.InvariantCulture, out var depth))
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5f, DerivedMeasures.Kilometre));
			return f;
		}
	}
}
