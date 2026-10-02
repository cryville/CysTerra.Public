using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.NowQuakeCENCInt.Model;
using Cryville.Measure;
using System;
using System.Collections.Generic;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.NowQuakeCENCInt.Features {
	public class NowQuakeCENCIntDataFeatureGenerator : IGenerator<WsData, Feature> {
		public Feature Generate(WsData e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var fIncludes = new List<Feature>();
			var f = new Feature {
				{ Is, ReportObservation },
				{ Subject, new Feature {
					{ Is, Earthquake },
					{ At, GenerateHypocenter(e) },
					{ Time, new DateTimeOffset(e.HappenTime, Local.TimeZoneOffset) },
					{ Magnitude, new QuantityInc(e.Magnitude, 0.05f, Units.Dimensionless) },
				} },
				{ TimeModified, new DateTimeOffset(e.UpdateTime, Local.TimeZoneOffset) },
				{ Includes, fIncludes },
			};
			if (e.MaxIntensity is float maxInt)
				f.Add(IntensityCSIS, new QuantityInc(maxInt, 0.05f, Units.Dimensionless));
			if (e.Stations is { } stations) {
				foreach (var station in stations) {
					fIncludes.Add(GenerateStation(station));
				}
			}
			return f;
		}

		static Feature GenerateHypocenter(WsData e) {
			var f = new Feature(new Point(e.Longitude, e.Latitude)) {
				{ Is, Hypocenter },
			};
			if (e.Hypocenter is string hypocenter)
				f.Add(Name, new Localized<string>(e.Hypocenter, Local.Culture));
			if (e.Depth is float depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5f, DerivedMeasures.Kilometre));
			return f;
		}

		static Feature GenerateStation(Station station) {
			return new Feature {
				{ Is, ReportObservation },
				{ At, new Feature(new Point(station.Longitude, station.Latitude)) {
					{ Is, ManMadeMonitoringStation },
					{ MonitoringSeismicActivity, true },
					{ Name, new Localized<string>(station.Name, Local.Culture) },
					{ Ref, station.StationId },
				} },
				{ IntensityCSIS, new QuantityInc(station.Intensity, 0.05f, Units.Dimensionless) },
				{ PGA, new QuantityInc(station.PGA, 0.05f, DerivedMeasures.CentimetrePerSecondSquared) },
				{ PGV, new QuantityInc(station.PGV, 0.05f, DerivedMeasures.CentimetrePerSecond) },
			};
		}
	}
}
