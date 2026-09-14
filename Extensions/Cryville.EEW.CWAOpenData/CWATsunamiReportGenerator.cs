using Cryville.Common.Compat;
using Cryville.EEW.CWAOpenData.Model;
using Cryville.EEW.Report;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cryville.EEW.CWAOpenData {
	public sealed partial class CWATsunamiReportGenerator : IContextedGenerator<Tsunami, IReportGeneratorContext, ReportModel> {
		readonly static TagTypeKey TagTsunamiWarningCWA = "TsunamiWarning:CWA";

		public ReportModel Generate(Tsunami e, IReportGeneratorContext? context, ref CultureInfo culture) {
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
			bool isInfo = false, isWarn = false;
			if (e.TsunamiWave is TsunamiWave tsunamiWave) {
				if (tsunamiWave.TsuStations is TsuStation[] stations && stations.Length > 0) {
					float? maxHeight = stations.Max(station => CWAOpenDataUtils.ToWaveHeight(station.WaveHeight));
					if (maxHeight is float h) {
						result.Properties.Add(new(TagTypeKeys.TsunamiHeight, res.GetStringRequired("PropertyMaxTsunamiHeight"), string.Format(culture, res.GetStringRequired("PropertyMaxTsunamiHeightValue"), h), context.SeverityScheme, h) { AccuracyOrder = 10 });
					}
					else {
						result.Properties.Add(new(TagTypeKeys.TsunamiHeight, res.GetStringRequired("PropertyMaxTsunamiHeight"), res.GetStringRequired("PropertyMaxTsunamiHeightValueUnknown"), -1) { AccuracyOrder = 10 });
					}
					isWarn = true;
				}
				else if (tsunamiWave.WarningAreas is WarningArea[] areas && areas.Length > 0) {
					result.Properties.Add(new(TagTsunamiWarningCWA, null, res.GetStringRequired("PropertyTsunamiWarning"), areas.Max(CWAOpenDataUtils.GetTsunamiWarningSeverity)) { AccuracyOrder = 70 });
					isInfo = true;
					result.InvalidatedTime = e.ValidTime.EndTime;
				}
			}
			else {
				result.Properties.Add(new(TagTsunamiWarningCWA, null, res.GetStringRequired("PropertyTsunamiMessage"), 0.5f) { AccuracyOrder = 70 });
				isInfo = true;
			}
			if (e.ReportType == "海嘯警報解除") {
				isInfo = true;
				isWarn = true;
			}
			if (e.EarthquakeInfo is EarthquakeInfo info) {
				CWAEarthquakeReportGenerator.ApplyEarthquakeInfo(culture, res, localFlag, result, info, context);
			}
			result.GroupKeys.Add(new ReportUnitKey(e.TsunamiNo, isInfo, isWarn));
			result.RevisionKey = new ReportRevisionKey(int.TryParse(ReportNoRegex().Match(e.ReportNo).Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int no) ? no : null);
			return result;
		}

#if NET7_0_OR_GREATER
		[GeneratedRegex(@"\d+")]
		private static partial Regex ReportNoRegex();
#else
		static readonly Regex r_ReportNoRegex = new(@"\d+");
		static Regex ReportNoRegex() => r_ReportNoRegex;
#endif

		sealed record ReportUnitKey(int TsunamiNo, bool IsInformational, bool IsWarning) : IReportUnitKey {
			public bool Equals(ReportUnitKey? obj) => obj is not null && TsunamiNo == obj.TsunamiNo;
			public override int GetHashCode() => TsunamiNo.GetHashCode();
			public bool IsCoveredBy(IReportUnitKey key)
				=> key is ReportUnitKey other
				&& TsunamiNo == other.TsunamiNo
				&& (!IsInformational || other.IsInformational)
				&& (!IsWarning || other.IsWarning);
		}
		sealed record ReportRevisionKey(int? ReportNo) : IReportRevisionKey {
			public int? Serial => ReportNo;
		}
	}
}
