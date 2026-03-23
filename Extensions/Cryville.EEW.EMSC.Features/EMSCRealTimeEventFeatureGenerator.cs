using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.Measure;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.EMSC.Features {
	public class EMSCRealTimeEventFeatureGenerator : IGenerator<EMSCRealTimeAction, Feature?> {
		public Feature? Generate(EMSCRealTimeAction e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			if (e.Event is not EMSCRealTimeEvent ev) {
				return null;
			}
			return new Feature {
				{ Is, Earthquake },
				{ Time, ev.Time },
				{
					At,
					new Feature(new Point(ev.Longitude, ev.Latitude)) {
						{ Is, Hypocenter },
						{ Name, new Localized<string>(ev.FlynnRegion, Local.Culture) },
						{ HypocenterDepth, new Quantity(ev.Depth, DerivedMeasures.Kilometre) },
					}
				},
				{
					ev.MagnitudeType switch {
						"mb" => MagnitudeBodyWave,
						"md" => MagnitudeDuration,
						"ml" => MagnitudeLocal,
						"mw" => MagnitudeMoment,
						_ => Magnitude,
					},
					new Quantity(ev.Magnitude, Units.Dimensionless)
				},
				{ Source, new Localized<string>(ev.Authority, CultureInfo.InvariantCulture) },
				{ TimeModified, ev.LastUpdate },
			};
		}
	}
}
