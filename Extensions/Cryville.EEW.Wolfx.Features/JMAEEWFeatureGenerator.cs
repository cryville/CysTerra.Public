using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.EEW.JMA;
using Cryville.EEW.JMA.Features;
using Cryville.EEW.Wolfx.Model;
using Cryville.Measure;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.Wolfx.Features {
	public class JMAEEWFeatureGenerator : IGenerator<JMAEEW, Feature?> {
		public Feature? Generate(JMAEEW e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			if (e.Status is JMAEEWStatus.GeneralCancellation or JMAEEWStatus.DrillingCancellation)
				return null;
			var fAreas = new Collection<Feature>();
			var f = new Feature {
				{ Is, ReportForecast },
				{ Subject, GenerateFromEarthquake(e) },
				{ Includes, fAreas },
				{ Source, new Localized<string>(e.PublishingOffice switch {
					JMAEEWPublishingOffice.Sapporo => "札幌管区気象台",
					JMAEEWPublishingOffice.Sendai => "仙台管区気象台",
					JMAEEWPublishingOffice.Tokyo => "気象庁本庁",
					JMAEEWPublishingOffice.Osaka => "大阪管区気象台",
					JMAEEWPublishingOffice.Fukuoka => "福岡管区気象台",
					_ => "気象庁本庁",
				}, Local.JapanCulture) },
				{ TimeModified, new DateTimeOffset(e.DateTime, Local.JapanTimeZoneOffset) },
			};
			if (e.MaxIntensity is string maxIntensity)
				f.Add(IntensityJMA, maxIntensity);
			foreach (var area in e.ForecastAreas)
				fAreas.Add(GenerateFromForecastArea(e, area));
			return f;
		}
		static Feature GenerateFromEarthquake(JMAEEW e) {
			bool hasLowHypocenterAccuracy = e.HypocenterAccuracy == JMAEEWHypocenterAccuracy.PSWaveLevelExcessionOrIPF1PointOrAssumedHypocenterElement;
			bool isHypocenterAssumed = hasLowHypocenterAccuracy && (e.Magnitude is null or 1.0f);
			var f = new Feature {
				{ Is, Earthquake },
				{ Ongoing, true },
				{ Time, new DateTimeOffset(e.OriginTime, Local.JapanTimeZoneOffset) },
				{ At, isHypocenterAssumed ? GenerateFromAssumedHypocenter(e) : GenerateFromHypocenter(e) },
			};
			if (e.Magnitude is float magnitude && !isHypocenterAssumed)
				f.Add(MagnitudeJMA, new QuantityInc(magnitude, 0.05f, Units.Dimensionless));
			return f;
		}
		static Feature GenerateFromAssumedHypocenter(JMAEEW e) {
			var f = new Feature(GetHypocenterPoint(e)) {
				{ Is, HypocenterAssumed },
			};
			GenerateFromHypocenterCode(e, f);
			return f;
		}
		static Feature GenerateFromHypocenter(JMAEEW e) {
			var f = new Feature(GetHypocenterPoint(e)) {
				{ Is, Hypocenter },
			};
			GenerateFromHypocenterCode(e, f);
			if (e.Depth is int depth)
				f.Add(HypocenterDepth, new QuantityInc(depth, 5f, DerivedMeasures.Kilometre));
			return f;
		}
		static Point? GetHypocenterPoint(JMAEEW e) {
			Point? point = null;
			if (e.Latitude is float lat && e.Longitude is float lon)
				point = new(lon, lat);
			return point;
		}
		static void GenerateFromHypocenterCode(JMAEEW e, Feature f) {
			if (e.HypocenterCode is string hypocenterCode) {
				using var resAreaEpicenter = JMAMessages.AreaEpicenter();
				f.Add(Ref, hypocenterCode);
				f.Add(Name, resAreaEpicenter.RootMessageStringSet.GetString(hypocenterCode));
			}
		}
		static Feature GetOrCreateAreaForecastLocalEarthquakeFeature(JMAEEWForecastArea area) {
			if (!JMAAreaForecastLocalEarthquakeFeatures.Instance.Areas.TryGetValue(area.Code, out var f))
				f = new(UnknownGeometry.Instance) {
					{ Is, PlaceInfoArea },
					{ AreaLevel, 5 },
					{ Ref, area.Code },
				};
			return f;
		}
		static Feature GenerateFromForecastArea(JMAEEW e, JMAEEWForecastArea area) {
			var f = new Feature {
				{ Is, ReportForecast },
				{ At, GetOrCreateAreaForecastLocalEarthquakeFeature(area) },
				{ IntensityJMA, area.Intensity1 },
			};
			if (area.ArrivalTime is DateTime rawArrivalTime && e.OriginTime is DateTime originTime) {
				DateTime arrivalTime = originTime.Date + rawArrivalTime.TimeOfDay;
				if (arrivalTime < originTime)
					arrivalTime += TimeSpan.FromDays(1);
				f.Add(Time, new DateTimeOffset(arrivalTime, Local.JapanTimeZoneOffset));
			}
			return f;
		}
	}
}
