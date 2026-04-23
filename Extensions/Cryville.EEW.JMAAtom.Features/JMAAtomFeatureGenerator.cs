using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.JMA;
using Cryville.EEW.JMA.Features;
using Cryville.EEW.JMAAtom.Model;
using Cryville.EEW.JMAAtom.Model.Seismology;
using Cryville.EEW.JMAAtom.Model.Volcanology;
using Cryville.Measure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using static Cryville.EEW.JMAAtom.Features.ExtraTagTypeKeys;
using static Cryville.EEW.TagTypeKeys;
using Tag = System.Collections.Generic.KeyValuePair<Cryville.EEW.TagTypeKey, object?>;

namespace Cryville.EEW.JMAAtom.Features {
	public class JMAAtomFeatureGenerator : IGenerator<JMAReport, Feature> {
		public Feature Generate(JMAReport e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature() {
				{ Is, TagTypeKeys.Report },
			};
			switch (e.Body) {
				case Model.Seismology.Body sBody:
					GenerateFromSeismologyBody(f, sBody);
					break;
				case Model.Volcanology.Body vBody:
					GenerateFromVolcanologyBody(f, vBody);
					break;
			}
			return f;
		}

		static void GenerateFromSeismologyBody(Feature f, Model.Seismology.Body body) {
			Collection<Feature>? fIncludes = null;
			if (body.Tsunami is Tsunami tsunami) {
				if (tsunami.Forecast is TsunamiDetail tsuForecast) {
					fIncludes = [];
					fIncludes.Add(GenerateFromTsunamiForecast(tsuForecast));
				}
				if (tsunami.Observation is TsunamiDetail tsuObservation) {
					fIncludes ??= [];
					fIncludes.Add(GenerateFromTsunamiObservation(tsuObservation));
				}
			}
			if (body.Intensity is Intensity intensity) {
				if (intensity.Observation is IntensityDetail observation) {
					fIncludes ??= [];
					fIncludes.Add(GenerateFromIntensityObservation(observation));
				}
			}
			if (body.EarthquakeInfo is EarthquakeInfo eqInfo) {
				fIncludes ??= [];
				fIncludes.Add(GenerateFromEarthquakeInfo(eqInfo));
			}
			if (fIncludes != null) {
				f.Add(Includes, fIncludes);
			}

			if (body.Earthquakes is Collection<Earthquake> eqs && eqs.Count > 0) {
				f.Add(Subject, (IReadOnlyCollection<Feature>)[.. eqs.Select(GenerateFromEarthquake)]);
			}
			if (!string.IsNullOrEmpty(body.Text)) {
				f.Add(Description, new Localized<string>(body.Text, Local.Culture));
			}
			Collection<LocalizableCollection<string>> comments = [];
			if (!string.IsNullOrEmpty(body.NextAdvisory)) {
				comments.Add([new Localized<string>(body.NextAdvisory, Local.Culture)]);
			}
			if (body.Comments is Comment comment) {
				if (comment.WarningComment is CommentForm warningComment) {
					comments.Add([new Localized<string>(warningComment.Text, Local.Culture)]);
				}
				if (comment.ForecastComment is CommentForm forecastComment) {
					comments.Add([new Localized<string>(forecastComment.Text, Local.Culture)]);
				}
				if (comment.VarComment is CommentForm varComment) {
					comments.Add([new Localized<string>(varComment.Text, Local.Culture)]);
				}
				if (!string.IsNullOrEmpty(comment.FreeFormComment)) {
					comments.Add([new Localized<string>(comment.FreeFormComment, Local.Culture)]);
				}
			}
			if (comments.Count > 0) {
				f.Add(TagTypeKeys.Comment, comments);
			}
		}

		static Feature GenerateFromTsunamiForecast(TsunamiDetail tsuForecast) {
			var areas = new Collection<Feature>();
			var f = new Feature() {
				{ Is, ReportForecast },
				{ At, JMAAreaTsunamiFeatures.Instance.Outline },
				{ Includes, areas }
			};
			foreach (var area in tsuForecast.Items) {
				if (GenerateFromTsunamiForecastArea(area) is Feature fArea) {
					areas.Add(fArea);
				}
			}
			return f;
		}

		static Feature GetOrCreateAreaTsunamiFeature(ForecastArea area) {
			if (!JMAAreaTsunamiFeatures.Instance.Areas.TryGetValue(area.Code, out var f))
				f = new(UnknownGeometry.Instance) {
					{ Is, PlaceInfoArea },
					{ AreaLevel, 4 },
					{ Ref, area.Code },
					{ Name, (LocalizableCollection<string>)[
						new Localized<string>(area.Name, Local.Culture),
					] },
				};
			return f;
		}

		static Feature GetOrCreatePointTsunamiFeature(TsunamiStation station) {
			if (!JMAPointTsunamiFeatures.Instance.Points.TryGetValue(station.Code, out var f))
				f = new(UnknownGeometry.Instance) {
					{ Is, ManMadeMonitoringStation },
					{ MonitoringTideGauge, true },
					{ Ref, station.Code },
					{ Name, (LocalizableCollection<string>)[
						new Localized<string>(station.Name, Local.Culture),
					] },
				};
			return f;
		}

		static Feature? GenerateFromTsunamiForecastArea(TsunamiItem area) {
			var fIncludes = new Collection<Feature>();
			Feature fArea = new() {
				{ Is, ReportForecast },
				{ At, GetOrCreateAreaTsunamiFeature(area.Area) },
				{ Severity, area.Category.Kind.Code switch {
					"52" or "53" => 1.25,
					[.., '0'] => -1,
					['5', ..] => 1,
					['6', ..] => 0.75,
					['7', ..] => 0,
					_ => -1,
				} },
				{ Includes, fIncludes },
			};
			if (area.FirstHeight is FirstHeight firstHeight) {
				fIncludes.Add(new() {
					{ Is, ReportForecast },
					{ Subject, TsunamiArrival },
					{ Time, firstHeight.Condition switch {
						"既に津波到達と推測" or "津波到達中と推測" => 0,
						"第１波の到達を確認" => -1,
						_ => firstHeight.ArrivalTime.Value,
					} },
				});
			}
			if (area.MaxHeight is MaxHeight maxHeight) {
				if (maxHeight.TsunamiHeight is MeasuredValue<float> h && !float.IsNaN(h.Value)) {
					fArea.Add(TsunamiHeight, GenerateFromTsunamiHeight(h));
				}
			}

			if (area.Stations is Collection<TsunamiStation> stations && stations.Count > 0) {
				foreach (var station in area.Stations) {
					if (GenerateFromTsunamiForecastStation(station) is Feature fStation) {
						fIncludes.Add(fStation);
					}
				}
			}
			return fArea;
		}

		static Feature? GenerateFromTsunamiForecastStation(TsunamiStation station) {
			var obs = new Collection<Feature>();
			Feature fStation = new() {
				{ Is, ReportForecast },
				{ At, GetOrCreatePointTsunamiFeature(station) },
				{ Includes, obs },
			};

			var sFirstHeight = station.FirstHeight;
			Feature fFirstHeight = new() {
				{ Is, ReportForecast },
				{ Subject, TsunamiArrival },
				{ Time, sFirstHeight.Condition switch {
					"既に津波到達と推測" or "津波到達中と推測" => 0,
					"第１波の到達を確認" => -1,
					_ => sFirstHeight.ArrivalTime.Value,
				} },
			};
			obs.Add(fFirstHeight);

			if (station.HighTideDateTime != default) {
				Feature fHighTide = new() {
					{ Is, ReportForecast },
					{ Subject, TideHigh },
					{ Time, station.HighTideDateTime.Value },
				};
				obs.Add(fHighTide);
			}
			return fStation;
		}

		static Feature GenerateFromTsunamiObservation(TsunamiDetail tsuObservation) {
			var areas = new Collection<Feature>();
			var f = new Feature() {
				{ Is, ReportObservation },
				{ At, JMAAreaTsunamiFeatures.Instance.Outline },
				{ Includes, areas },
			};
			foreach (var area in tsuObservation.Items) {
				if (GenerateFromTsunamiObservationArea(area) is Feature fArea) {
					areas.Add(fArea);
				}
			}
			return f;
		}

		static Feature? GenerateFromTsunamiObservationArea(TsunamiItem area) {
			var stations = new Collection<Feature>();
			Feature fArea = new() {
				{ Is, ReportObservation },
				{ At, GetOrCreateAreaTsunamiFeature(area.Area) },
				{ Includes, stations },
			};
			foreach (var station in area.Stations) {
				if (GenerateFromTsunamiObservationStation(station) is Feature fStation) {
					stations.Add(fStation);
				}
			}
			return fArea;
		}

		static Feature? GenerateFromTsunamiObservationStation(TsunamiStation station) {
			var obs = new Collection<Feature>();
			Feature fStation = new() {
				{ Is, ReportObservation },
				{ At, GetOrCreatePointTsunamiFeature(station) },
				{ Includes, obs },
			};

			Feature fFirstHeight = new() {
				{ Is, ReportObservation },
				{ Subject, TsunamiArrival },
			};
			var firstHeight = station.FirstHeight;
			// TODO 欠測 string[] firstHeightConditions = firstHeight.Condition?.Split('\x3000') ?? [];
			if (firstHeight.ArrivalTime != default)
				fFirstHeight.Add(Time, firstHeight.ArrivalTime.Value);
			int? initial = firstHeight.Initial switch {
				"引き" => -1,
				"押し" => 1,
				_ => null,
			};
			if (initial != null)
				fFirstHeight.Add(Polarity, initial);
			if (fFirstHeight.Count > 2)
				obs.Add(fFirstHeight);

			if (station.MaxHeight is MaxHeight maxHeight) {
				if (maxHeight.DateTime != default)
					fStation.Add(Time, maxHeight.DateTime.Value);
				string[] maxHeightConditions = maxHeight.Condition?.Split('\x3000') ?? [];
				// TODO 欠測
				if (maxHeightConditions.Contains("微弱"))
					fStation.Add(TsunamiHeight, new QuantityInc(0, 0.05, Units.Metre));
				else if (maxHeight.TsunamiHeight is MeasuredValue<float> h && !float.IsNaN(h.Value)) {
					fStation.Add(TsunamiHeight, GenerateFromTsunamiHeight(h));
				}
			}
			return fStation;
		}

		static object GenerateFromTsunamiHeight(MeasuredValue<float> h) {
			var q = new QuantityInc(h.Value, 0.05f, Units.Metre);
			return h.Description switch {
				[.., '超'] => new Interval<QuantityInc>(q, new(double.PositiveInfinity, Units.Metre), IntervalEndpointTypes.LeftOpen),
				[.., '以', '上'] => new Interval<QuantityInc>(q, new(double.PositiveInfinity, Units.Metre)),
				[.., '未', '満'] => new Interval<QuantityInc>(new(0d, Units.Metre), q, IntervalEndpointTypes.RightOpen),
				_ => q,
			};
		}

		static Feature GetOrCreateAreaInformationPrefectureEarthquakeFeature(IntensityPref pref) {
			return new(UnknownGeometry.Instance) {
				{ Is, PlaceInfoArea },
				{ AreaLevel, 4 },
				{ Ref, pref.Code },
				{ Name, (LocalizableCollection<string>)[
					new Localized<string>(pref.Name, Local.Culture),
				] },
			};
		}

		static Feature GetOrCreateAreaForecastLocalEarthquakeFeature(IntensityArea area) {
			if (!JMAAreaForecastLocalEarthquakeFeatures.Instance.Areas.TryGetValue(area.Code, out var f))
				f = new(UnknownGeometry.Instance) {
					{ Is, PlaceInfoArea },
					{ AreaLevel, 5 },
					{ Ref, area.Code },
					{ Name, (LocalizableCollection<string>)[
						new Localized<string>(area.Name, Local.Culture),
					] },
				};
			return f;
		}

		static Feature GetOrCreateAreaInformationCityQuakeFeature(IntensityCity city) {
			return new(UnknownGeometry.Instance) {
				{ Is, PlaceInfoArea },
				{ AreaLevel, 7 },
				{ Ref, city.Code },
				{ Name, (LocalizableCollection<string>)[
					new Localized<string>(city.Name, Local.Culture),
				] },
			};
		}

		static Feature GetOrCreatePointSeismicIntensityFeature(IntensityStation station) {
			if (!JMAPointSeismicIntensityFeatures.Instance.Points.TryGetValue(station.Code, out var f))
				f = new(UnknownGeometry.Instance) {
					{ Is, ManMadeMonitoringStation },
					{ MonitoringTideGauge, true },
					{ Ref, station.Code },
					{ Name, (LocalizableCollection<string>)[
						new Localized<string>(station.Name, Local.Culture),
					] },
				};
			return f;
		}

		static Feature GenerateFromIntensityObservation(IntensityDetail observation) {
			var f = new Feature() {
				{ Is, ReportObservation },
			};
			if (!string.IsNullOrEmpty(observation.MaxInt)) f.Add(IntensityJMA, observation.MaxInt);
			if (!string.IsNullOrEmpty(observation.MaxLgInt)) f.Add(IntensityJMALPGM, observation.MaxLgInt);
			if (observation.Prefs is Collection<IntensityPref> prefs && prefs.Count > 0) {
				var fPrefs = new Collection<Feature>();
				foreach (var pref in prefs) {
					fPrefs.Add(GenerateFromIntensityObservationPref(pref));
				}
				f.Add(Includes, fPrefs);
			}
			return f;
		}

		static Feature GenerateFromIntensityObservationPref(IntensityPref pref) {
			Feature fPref = new() {
				{ Is, ReportObservation },
				{ At, GetOrCreateAreaInformationPrefectureEarthquakeFeature(pref) },
			};
			if (!string.IsNullOrEmpty(pref.MaxInt)) fPref.Add(IntensityJMA, pref.MaxInt);
			if (!string.IsNullOrEmpty(pref.MaxLgInt)) fPref.Add(IntensityJMALPGM, pref.MaxLgInt);
			if (pref.Areas is Collection<IntensityArea> areas && areas.Count > 0) {
				var fAreas = new Collection<Feature>();
				foreach (var area in areas) {
					fAreas.Add(GenerateFromIntensityObservationArea(area));
				}
				fPref.Add(Includes, fAreas);
			}
			return fPref;
		}

		static Feature GenerateFromIntensityObservationArea(IntensityArea area) {
			Feature fArea = new() {
				{ Is, ReportObservation },
				{ At, GetOrCreateAreaForecastLocalEarthquakeFeature(area) },
			};
			if (!string.IsNullOrEmpty(area.MaxInt)) fArea.Add(IntensityJMA, area.MaxInt);
			if (!string.IsNullOrEmpty(area.MaxLgInt)) fArea.Add(IntensityJMALPGM, area.MaxLgInt);
			Collection<Feature>? fIncludes = null;
			if (area.Cities is Collection<IntensityCity> cities && cities.Count > 0) {
				fIncludes = [];
				foreach (var city in cities) {
					fIncludes.Add(GenerateFromIntensityObservationCity(city));
				}
			}
			if (area.IntensityStations is Collection<IntensityStation> stations && stations.Count > 0) {
				fIncludes ??= [];
				foreach (var station in stations) {
					fIncludes.Add(GenerateFromIntensityStation(station));
				}
			}
			if (fIncludes != null)
				fArea.Add(Includes, fIncludes);
			return fArea;
		}

		static Feature GenerateFromIntensityObservationCity(IntensityCity city) {
			Feature fCity = new() {
				{ Is, ReportObservation },
				{ At, GetOrCreateAreaInformationCityQuakeFeature(city) },
			};
			if (!string.IsNullOrEmpty(city.MaxInt)) fCity.Add(IntensityJMA, city.MaxInt);
			if (!string.IsNullOrEmpty(city.MaxLgInt)) fCity.Add(IntensityJMALPGM, city.MaxLgInt);
			if (city.IntensityStations is Collection<IntensityStation> stations && stations.Count > 0) {
				var fStations = new Collection<Feature>();
				foreach (var station in stations) {
					fStations.Add(GenerateFromIntensityStation(station));
				}
				fCity.Add(Includes, fStations);
			}
			return fCity;
		}

		static Feature GenerateFromIntensityStation(IntensityStation station) {
			Feature fStation = new() {
				{ Is, ReportObservation },
				{ At, GetOrCreatePointSeismicIntensityFeature(station) },
			};
			if (!string.IsNullOrEmpty(station.Intensity)) {
				if (station.Intensity == "震度５弱以上未入電") {
					fStation.Add(IntensityJMA, new Interval<double>(4.5, double.PositiveInfinity));
					// TODO 欠測
				}
				else fStation.Add(IntensityJMA, station.Intensity);
			}
			if (!string.IsNullOrEmpty(station.LgInt)) fStation.Add(IntensityJMALPGM, station.LgInt);
			if (station.Sva is Sva sva) fStation.Add(ExtraTagTypeKeys.Sva, new Quantity(sva.Value, DerivedMeasures.CentimetrePreSecond));

			var fIncludes = new Collection<Feature>();
			foreach (var period in FullGroupJoin(
				station.LgIntPerPeriods ?? (IEnumerable<LgIntPerPeriod>)[],
				station.SvaPerPeriods ?? (IEnumerable<SvaPerPeriod>)[],
				i => i.PeriodicBand,
				i => i.PeriodicBand,
				(k, i, j) => (Key: k, LgInt: i.Single(), Sva: j.Single())
			)) {
				Feature fInclude = new() {
					{ Is, ReportObservation },
					{ SeismicWavePeriod, new Interval<Quantity>(new(Math.Max(1.5, period.Key), Units.Second), new(Math.Min(8.0, period.Key + 1), Units.Second), IntervalEndpointTypes.RightOpen) },
				};
				if (period.LgInt is LgIntPerPeriod iLgInt)
					fInclude.Add(IntensityJMALPGM, iLgInt.Value);
				if (period.Sva is SvaPerPeriod iSva)
					fInclude.Add(ExtraTagTypeKeys.Sva, new QuantityInc(iSva.Value, 0.05f, DerivedMeasures.CentimetrePreSecond));
				fIncludes.Add(fInclude);
			}
			if (fIncludes.Count > 0)
				fStation.Add(Includes, fIncludes);

			static IEnumerable<TResult> FullGroupJoin<TFirst, TSecond, TKey, TResult>(
				IEnumerable<TFirst> first,
				IEnumerable<TSecond> second,
				Func<TFirst, TKey> firstKeySelector,
				Func<TSecond, TKey> secondKeySelector,
				Func<TKey, IEnumerable<TFirst>, IEnumerable<TSecond>, TResult> resultSelector
			) {
				var alookup = first.ToLookup(firstKeySelector);
				var blookup = second.ToLookup(secondKeySelector);

				foreach (var a in alookup) {
					yield return resultSelector(a.Key, a, blookup[a.Key]);
				}

				foreach (var b in blookup) {
					if (alookup.Contains(b.Key))
						continue;
					yield return resultSelector(b.Key, Enumerable.Empty<TFirst>(), b);
				}
			}

			return fStation;
		}

		static Feature GenerateFromEarthquake(Earthquake earthquake) {
			var hypocenter = earthquake.Hypocenter;
			var area = hypocenter.Area;
			return new() {
				{ Is, TagTypeKeys.Earthquake },
				{ At, (IReadOnlyCollection<Feature>)[.. area.Coordinates.Select(coordinates => {
					var f = new Feature(coordinates.Description != "震源要素不明" ? new Point(coordinates.Longitude, coordinates.Latitude) : UnknownGeometry.Instance) {
						{ Is, TagTypeKeys.Hypocenter },
						{ Name, (LocalizableCollection<string>)[
							new Localized<string>(area.Name, Local.Culture),
							JMAMessages.AreaEpicenter().RootMessageStringSet.GetString(area.Code.Value),
						] },
						{ Ref, area.Code.Value },
					};
					if (coordinates.Description != "震源要素不明" && coordinates.Height is double h) {
						object depthValue;
						if (coordinates.Description.EndsWith("ごく浅い", StringComparison.OrdinalIgnoreCase))
							depthValue = new Interval<QuantityInc>(new(0d, Units.Metre), new(5000d, 5000d, Units.Metre), IntervalEndpointTypes.RightOpen);
						else if (coordinates.Description.EndsWith("深さは７００ｋｍ以上", StringComparison.OrdinalIgnoreCase))
							depthValue = new Interval<QuantityInc>(new(700000d, 5000d, Units.Metre), new(double.PositiveInfinity, Units.Metre));
						else
							depthValue = new QuantityInc(-h, coordinates.Type == "震源位置（度分）" ? 500 : 5000, Units.Metre);
						f.Add(HypocenterDepth, depthValue);
					}
					if (!string.IsNullOrEmpty(hypocenter.Source)) {
						f.Add(Source, hypocenter.Source);
					}
					return f;
				})] },
				{ Time, earthquake.OriginTime.Value },
				GenerateFromMagnitude(earthquake.Magnitudes.Single()),
			};
		}

		static Tag GenerateFromMagnitude(Magnitude magnitude) {
			return new(
				magnitude.Type switch {
					"Mj" => MagnitudeJMA,
					_ => TagTypeKeys.Magnitude
				},
				float.IsNaN(magnitude.Value)
					? magnitude.Description switch {
						"Ｍ８を超える巨大地震" => new Interval<QuantityInc>(new(8.0, 0.05, Units.Dimensionless), new(double.PositiveInfinity, Units.Dimensionless), IntervalEndpointTypes.LeftOpen),
						_ => null,
					}
					: new QuantityInc(magnitude.Value, 0.05f, Units.Dimensionless)
			);
		}

		static Feature GenerateFromEarthquakeInfo(EarthquakeInfo eqInfo) {
			Feature f = new() {
				{ Is, TagTypeKeys.Report },
			};
			if (!string.IsNullOrEmpty(eqInfo.Text)) {
				f.Add(Description, (LocalizableCollection<string>)[new Localized<string>(eqInfo.Text, Local.Culture)]);
			}
			if (!string.IsNullOrEmpty(eqInfo.Appendix)) {
				f.Add(TagTypeKeys.Comment, (LocalizableCollection<string>)[new Localized<string>(eqInfo.Appendix, Local.Culture)]);
			}
			return f;
		}

		static void GenerateFromVolcanologyBody(Feature f, Model.Volcanology.Body body) {
			var i = new Collection<Feature>();
			f[Includes] = i;
			if (body.AshInfos is AshInfos ashInfos) {
				int index = ashInfos.Items.Count;
				foreach (var ashInfo in ashInfos.Items.OrderByDescending(i => i.EndTime)) {
					foreach (var item in ashInfo.Items) {
						var kind = item.Kind;
						if (kind.Property is VolcanoProperty prop) {
							// TODO
						}
					}
				}
			}
			foreach (var info in body.VolcanoInfo) {
				foreach (var item in info.Items) {
					if (item.Areas.CodeType == "火山名") {
						foreach (var area in item.Areas.Items) {
							// TODO
						}
					}
				}
			}
		}
	}
}
