using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.NowQuakeCENCInt.Model;
using Cryville.Measure;
using System;
using System.Collections.Generic;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.NowQuakeCENCInt.Features {
	public class NowQuakeCENCIntV2FeatureGenerator : IGenerator<V2Report, Feature> {
		public Feature Generate(V2Report e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var data = e.Data;
			var ev = data.Event;
			var f = new Feature {
				{ Is, ReportObservation },
				{ Subject, GenerateEarthquake(ev) },
			};
			if (ev.ReportTime is DateTimeOffset reportTime)
				f.Add(TimeModified, reportTime);
			if (data.Details.MaximumIntensity is double maxInt)
				f.Add(IntensityCSIS, new QuantityInc(maxInt, 0.05, Units.Dimensionless));
			if (data.Stations is { } stations) {
				var fIncludes = new List<Feature>();
				f.Add(Includes, fIncludes);
				foreach (var station in stations) {
					fIncludes.Add(GenerateStation(station));
				}
			}
			return f;
		}

		static Feature GenerateEarthquake(Event e) {
			var f = new Feature {
				{ Is, Earthquake },
				{ At, GenerateHypocenter(e) },
			};
			if (e.OriginTime is DateTimeOffset originTime)
				f.Add(Time, originTime);
			if (e.Magnitude is double magnitude)
				f.Add(Magnitude, new QuantityInc(magnitude, 0.05, Units.Dimensionless));
			return f;
		}

		static Feature GenerateHypocenter(Event e) {
			var epicenter = e.Epicenter;
			Point? point = null;
			if (epicenter.Latitude is double latitude && epicenter.Longitude is double longitude)
				point = new Point(longitude, latitude);
			var f = new Feature(point) {
				{ Is, Hypocenter },
			};
			if (epicenter.Name is string epicenterName)
				f.Add(Name, new Localized<string>(epicenterName, Local.Culture));
			if (e.Depth is double depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 0.5, DerivedMeasures.Kilometre));
			return f;
		}

		static Feature GenerateStation(V2Station station) {
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
