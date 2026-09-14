using Cryville.Common.Compat;
using Cryville.EEW.CWA;
using Cryville.EEW.CWAOpenData.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System.Collections.Generic;
using System.Globalization;
using static Cryville.EEW.CWAOpenData.Features.ExtraTagTypeKeys;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.CWAOpenData.Features {
	public class CWAEarthquakeFeatureGenerator : IGenerator<Earthquake, Feature> {
		public Feature Generate(Earthquake e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, ReportObservation },
				{ Subject, GenerateFromEarthquakeInfo(e.EarthquakeInfo) },
				{ TimeModified, e.IssueTime },
			};
			if (e.Intensity is { } intensity) {
				if (intensity.ShakingAreas is { } areas && areas.Length > 0) {
					var fAreas = new List<Feature>();
					f.Add(Includes, fAreas);
					foreach (var area in areas) {
						if (area.InfoStatus != "observe")
							continue;
						fAreas.Add(GenerateFromShakingArea(area));
					}
				}
			}
			return f;
		}

		internal static Feature GenerateFromEarthquakeInfo(EarthquakeInfo earthquakeInfo) {
			var f = new Feature {
				{ Is, TagTypeKeys.Earthquake },
				{ Source, new Localized<string>(earthquakeInfo.Source, Local.Culture) },
			};
			if (earthquakeInfo.Epicenter is { } epicenter) {
				CWAOpenDataUtils.GetCoordinates(epicenter, out float lat, out float lon);
				f.Add(At, new Feature(new Point(lon, lat)) {
					{ Is, Hypocenter },
					{ Name, new Localized<string>(epicenter.Location, Local.Culture) },
					{ HypocenterDepth, new QuantityInc(earthquakeInfo.FocalDepth, earthquakeInfo.Source == "中央氣象署" ? 0.05f : 0.5f, DerivedMeasures.Kilometre) },
				});
			}
			if (earthquakeInfo.EarthquakeMagnitude is { } magnitude) {
				f.Add(
					magnitude.MagnitudeType switch {
						"芮氏規模" => MagnitudeRichter,
						_ => Magnitude,
					},
					new QuantityInc(magnitude.MagnitudeValue, 0.05f, Units.Dimensionless)
				);
			}
			return f;
		}

		static Feature GenerateFromShakingArea(ShakingArea area) {
			var f = new Feature {
				{ Is, ReportObservation },
				{ At, new Feature(UnknownGeometry.Instance) {
					{ Is, PlaceInfoArea },
					{ AreaLevel, 4 },
					{ Subject, TagTypeKeys.Earthquake },
					{ Name, new Localized<string>(area.CountyName, Local.Culture) },
				} },
				{ IntensityCWASIS, CWAMessageUtils.ToNormalizedIntensity(area.AreaIntensity) },
			};
			if (area.EqStations is { } eqStations && eqStations.Length > 0) {
				var fStations = new List<Feature>();
				f.Add(Includes, fStations);
				foreach (var station in eqStations) {
					fStations.Add(GenerateFromEqStation(station));
				}
			}
			return f;
		}

		static Feature GenerateFromEqStation(EqStation station) {
			var f = new Feature {
				{ Is, ReportObservation },
				{ At, new Feature(new Point(station.StationLongitude, station.StationLatitude)) {
					{ Is, ManMadeMonitoringStation },
					{ MonitoringSeismicActivity, true },
					{ Name, new Localized<string>(station.StationName, Local.Culture) },
					{ Ref, station.StationID },
				} },
				{ IntensityCWASIS, CWAMessageUtils.ToNormalizedIntensity(station.SeismicIntensity) },
			};
			if (station.PGA is { } pga)
				f.Add(PGACWASIS, new QuantityInc(pga.IntScaleValue, 0.005f, DerivedMeasures.Gal));
			if (station.PGV is { } pgv)
				f.Add(PGVCWASIS, new QuantityInc(pgv.IntScaleValue, 0.005f, DerivedMeasures.Kine));
			return f;
		}
	}
}
