using Cryville.Common.Compat;
using Cryville.EEW.CWAOpenData.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System.Collections.Generic;
using System.Globalization;
using static Cryville.EEW.CWAOpenData.Features.ExtraTagTypeKeys;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.CWAOpenData.Features {
	public class CWATsunamiFeatureGenerator : IGenerator<Tsunami, Feature> {
		public Feature Generate(Tsunami e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, TagTypeKeys.Report },
				{ Subject, CWAEarthquakeFeatureGenerator.GenerateFromEarthquakeInfo(e.EarthquakeInfo) },
				{ TimeModified, e.IssueTime },
			};
			if (e.TsunamiWave is { } tsunamiWave) {
				List<Feature>? fIncludes = null;
				if (tsunamiWave.WarningAreas is { } warningAreas && warningAreas.Length > 0) {
					fIncludes = [];
					var fAreas = new List<Feature>();
					fIncludes.Add(new Feature {
						{ Is, ReportForecast },
						{ At, CWATsunamiAreaFeatures.Instance.Outline },
						{ Includes, fAreas },
					});
					foreach (var area in warningAreas) {
						fAreas.Add(GenerateFromWarningArea(area));
					}
				}
				if (tsunamiWave.TsuStations is { } tsuStations && tsuStations.Length > 0) {
					fIncludes ??= [];
					var fStations = new List<Feature>();
					fIncludes.Add(new Feature {
						{ Is, ReportObservation },
						{ Includes, fStations },
					});
					foreach (var station in tsuStations) {
						fStations.Add(GenerateFromTsuStation(station));
					}
				}
				if (fIncludes != null) {
					f.Add(Includes, fIncludes);
				}
			}
			return f;
		}

		static Feature GetOrCreateWarningAreaFeature(WarningArea area) {
			if (!CWATsunamiAreaFeatures.Instance.Areas.TryGetValue(area.AreaName, out var f))
				f = new(UnknownGeometry.Instance) {
					{ Is, PlaceInfoArea },
					{ AreaLevel, 3 },
					{ Subject, TagTypeKeys.Tsunami },
					{ Name, (LocalizableCollection<string>)[
						new Localized<string>(area.AreaName, Local.Culture),
					] },
				};
			return f;
		}

		static Feature GenerateFromWarningArea(WarningArea area) {
			return new Feature {
				{ Is, ReportForecast },
				{ At, GetOrCreateWarningAreaFeature(area) },
				{ Severity, CWAOpenDataUtils.GetTsunamiWarningSeverity(area) },
				{ TsunamiHeight, area.WaveHeight switch {
					"大於6公尺" => new Interval<QuantityInc>(new(6d, Units.Metre), new(double.PositiveInfinity, Units.Metre), IntervalEndpointTypes.LeftOpen),
					"3至6公尺" => new Interval<QuantityInc>(new(3d, Units.Metre), new(6d, Units.Metre)),
					"1至3公尺" => new Interval<QuantityInc>(new(1d, Units.Metre), new(3d, Units.Metre), IntervalEndpointTypes.RightOpen),
					"小於1公尺" => new Interval<QuantityInc>(new(0d, Units.Metre), new(1d, Units.Metre), IntervalEndpointTypes.RightOpen),
					"0.3至1公尺" => new Interval<QuantityInc>(new(0.3d, Units.Metre), new(1d, Units.Metre), IntervalEndpointTypes.RightOpen),
					"小於0.3公尺" => new Interval<QuantityInc>(new(0d, Units.Metre), new(0.3d, Units.Metre), IntervalEndpointTypes.RightOpen),
					_ => null,
				} },
				{ Includes, (List<Feature>)[
					new Feature {
						{ Is, ReportForecast },
						{ Subject, TsunamiArrival },
						{ Time, area.ArrivalTime },
					},
				] },
			};
		}

		static Feature GenerateFromTsuStation(TsuStation station) {
			Geometry point;
			if (station.StationLongitude == 0 && station.StationLatitude == 0)
				point = UnknownGeometry.Instance;
			else
				point = new Point(station.StationLongitude, station.StationLatitude);

			var f = new Feature {
				{ Is, ReportObservation },
				{ At, new Feature(point) {
					{ Is, ManMadeMonitoringStation },
					{ MonitoringTideGauge, true },
					{ Name, new Localized<string>(station.StationName, Local.Culture) },
					{ Ref, station.StationID },
				} },
				{ Includes, (List<Feature>)[
					new Feature {
						{ Is, ReportObservation },
						{ Subject, TsunamiArrival },
						{ Time, station.ArrivalTime },
					},
				] },
			};
			if (CWAOpenDataUtils.ToWaveHeight(station.WaveHeight) is float waveHeight)
				f.Add(TsunamiHeight, new QuantityInc(waveHeight, 0.005f, Units.Metre));
			return f;
		}
	}
}
