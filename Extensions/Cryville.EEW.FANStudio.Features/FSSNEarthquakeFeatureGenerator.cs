using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.FANStudio.Features {
	public class FSSNEarthquakeFeatureGenerator : IGenerator<FSSNEarthquake, Feature> {
		public Feature Generate(FSSNEarthquake e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			return new Feature {
				{ Is, Earthquake },
				{ Time, new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) },
				{ At, new Feature(new Point(e.Longitude, e.Latitude)) {
					{ Is, Hypocenter },
					{ Name, (LocalizableCollection<string>)[
						new Localized<string>(e.PlaceName, Local.UnitedStatesCulture),
						new Localized<string>(e.PlaceNameZh, Local.Culture),
					] },
					{ HypocenterDepth, new QuantityInc(e.Depth, 0.5f, DerivedMeasures.Kilometre) },
				} },
				{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
				{ TimeModified, new DateTimeOffset(e.CreateTime, Local.TimeZoneOffset) },
				// TODO EvaluationMode
			};
		}
	}
}
