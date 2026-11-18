using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.NowQuakeCENCInt.Model;
using Cryville.Measure;
using System;
using System.Collections.Generic;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.NowQuakeCENCInt.Features {
	[Obsolete("Use the V2 protocol instead.")]
	public class NowQuakeCENCIntDataFeatureGenerator : IGenerator<WsData, Feature> {
		public Feature Generate(WsData e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, ReportObservation },
				{ Subject, GenerateEarthquake(e) },
			};
			if (e.UpdateTime is DateTime updateTime)
				f.Add(TimeModified, new DateTimeOffset(updateTime, Local.TimeZoneOffset));
			if (e.Maxintensity is double maxInt)
				f.Add(IntensityCSIS, new QuantityInc(maxInt, 0.05, Units.Dimensionless));
			if (e.Stations is { } stations) {
				var fIncludes = new List<Feature>();
				f.Add(Includes, fIncludes);
				foreach (var station in stations) {
					fIncludes.Add(GenerateStation(station));
				}
			}
			return f;
		}

		static Feature GenerateEarthquake(WsData e) {
			var f = new Feature {
				{ Is, Earthquake },
				{ At, GenerateHypocenter(e) },
			};
			if (e.HappenTime is DateTime happenTime)
				f.Add(Time, new DateTimeOffset(happenTime, Local.TimeZoneOffset));
			if (e.Magnitude is double magnitude)
				f.Add(Magnitude, new QuantityInc(magnitude, 0.05, Units.Dimensionless));
			return f;
		}

		static Feature GenerateHypocenter(WsData e) {
			Point? point = null;
			if (e.Latitude is double latitude && e.Longitude is double longitude)
				point = new Point(longitude, latitude);
			var f = new Feature(point) {
				{ Is, Hypocenter },
			};
			if (e.Hypocenter is string hypocenter)
				f.Add(Name, new Localized<string>(hypocenter, Local.Culture));
			if (e.Depth is double depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5, DerivedMeasures.Kilometre));
			return f;
		}

		static Feature GenerateStation(Station station) {
			Point? point = null;
			if (station.Latitude is double latitude && station.Longitude is double longitude)
				point = new Point(longitude, latitude);
			var fAt = new Feature(point) {
				{ Is, ManMadeMonitoringStation },
				{ MonitoringSeismicActivity, true },
				{ Name, new Localized<string>(station.Name, Local.Culture) },
				{ Ref, station.Stationid },
			};
			var f = new Feature {
				{ Is, ReportObservation },
				{ At, fAt },
			};
			if (station.Int is double @int)
				f.Add(IntensityCSIS, new QuantityInc(@int, 0.05, Units.Dimensionless));
			if (station.Pga is double pga)
				f.Add(PGA, new QuantityInc(pga, 0.05, DerivedMeasures.CentimetrePerSecondSquared));
			if (station.Pgv is double pgv)
				f.Add(PGV, new QuantityInc(pgv, 0.05, DerivedMeasures.CentimetrePerSecond));
			return f;
		}
	}
}
