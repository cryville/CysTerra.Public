using Cryville.Common.Compat;
using Cryville.EEW.CWA;
using Cryville.EEW.CWAOpenData.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;
using System.Linq;

namespace Cryville.EEW.CWAOpenData {
	public sealed class CWAEarthquakeReportGenerator : IContextedGenerator<Earthquake, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(Earthquake e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			bool localFlag = culture.Equals(Local.Culture);
			var result = new ReportModel {
				Title = localFlag ? e.ReportType : res.GetStringSetRequired("Title").GetString(e.ReportType) ?? e.ReportType,
				Source = res.GetStringRequired("AuthorityName"),
				UtcIssueTime = e.IssueTime.UtcDateTime,
				TimeZone = Local.TimeZone,
			};
			if (e.Intensity is Intensity intensity) {
				string? maxIntensity = FindMaxIntensity(intensity);
				result.Properties.Add(new(TagTypeKeys.IntensityCWASIS, res.GetStringRequired("PropertyMaxIntensity"), CWAMessageUtils.ToLongDisplayIntensity(maxIntensity, culture) ?? "?", context.SeverityScheme, CWAMessageUtils.ToNormalizedIntensity(maxIntensity)) { AccuracyOrder = 10 });
			}
			if (e.EarthquakeInfo is EarthquakeInfo info) {
				ApplyEarthquakeInfo(culture, res, localFlag, result, info, context);
			}
			return result;
		}

		internal static void ApplyEarthquakeInfo(CultureInfo culture, IMessageStringSet res, bool localFlag, ReportModel result, EarthquakeInfo info, IReportGeneratorContext context) {
			var epicenter = info.Epicenter;
			CWAOpenDataUtils.GetCoordinates(epicenter, out float lat, out float lon);
			if (epicenter != null) {
				context.NameLocationTo(result, lat, lon, Local.Culture, culture);
			}
			if (result.Location == null) {
				result.Location = CWAOpenDataUtils.ToShortLocation(info.Epicenter?.Location);
				result.LocationSpecificity = GetSpecificity(info.Epicenter?.Location);
			}
			result.Time = info.OriginTime;
			if (info.EarthquakeMagnitude is EarthquakeMagnitude magnitude) {
				result.Properties.Add(new(
					magnitude.MagnitudeType switch {
						"芮氏規模" => TagTypeKeys.MagnitudeRichter,
						_ => TagTypeKeys.Magnitude,
					},
#pragma warning disable IDE0079 // False report
#pragma warning disable CA1508
					localFlag ? (magnitude.MagnitudeType ?? "M") : res.GetStringSetRequired("PropertyMagnitude").GetStringOrDefault(magnitude.MagnitudeType ?? ""),
#pragma warning restore CA1508
#pragma warning restore IDE0079
					magnitude.MagnitudeValue.ToString("F1", culture),
					context.SeverityScheme,
					magnitude.MagnitudeValue
				) { AccuracyOrder = 10 });
				if (epicenter != null) {
					result.GroupKeys.Add(new HypocenterGroupKey(lat, lon, info.OriginTime.UtcDateTime, magnitude.MagnitudeValue, info.FocalDepth));
				}
			}
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), info.FocalDepth), context.SeverityScheme, info.FocalDepth) { AccuracyOrder = 10 });
		}

		public static string? FindMaxIntensity(Intensity intensity) {
			ThrowHelper.ThrowIfNull(intensity);
			return intensity.ShakingAreas.OrderByDescending(area => CWAMessageUtils.KeyIntensity(area.AreaIntensity)).FirstOrDefault()?.AreaIntensity;
		}

		static readonly string[] _localAreas = [
			"臺北", "新北", "桃園", "臺中", "臺南", "高雄", "基隆", "新竹", "嘉義",
			"苗栗", "彰化", "南投", "雲林", "屏東", "宜蘭", "花蓮", "臺東", "澎湖", "金門", "連江",
		];
		static int GetSpecificity(string? location) {
			if (location is null) return 0;
			foreach (var area in _localAreas) {
				if (location.StartsWith(area, StringComparison.Ordinal)) {
					return 8;
				}
			}
			return 3;
		}
	}
}
