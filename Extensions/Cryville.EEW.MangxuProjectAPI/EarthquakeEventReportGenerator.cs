using Cryville.Common.Compat;
using Cryville.EEW.MangxuProjectAPI.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Cryville.EEW.MangxuProjectAPI {
	public partial class EarthquakeEventReportGenerator : IContextedGenerator<EarthquakeEvent, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(EarthquakeEvent e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			bool localFlag = culture.Equals(Local.Culture);
			ExtractTitleAndSource(e.Agency, out string title, out string? source);
			if (!localFlag)
				title = res.GetStringSetRequired("Title").GetStringOrDefault(title);
			var result = new ReportModel {
				Title = title,
				Source = source,
				Time = new DateTimeOffset(e.TimeUtc, TimeSpan.Zero),
			};
			result.GroupKeys.Add(new ReportUnitKey(e.Id));
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, e.TimeUtc, e.Magnitude, e.DepthKm));
			result.RevisionKey = new ReportRevisionKey();

			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture)) {
				result.Location = e.Location;
				result.LocationSpecificity = 3;
			}

			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 10 });
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.DepthKm), context.SeverityScheme, e.DepthKm) { AccuracyOrder = 10 });

			return result;
		}

#if NET7_0_OR_GREATER
		[GeneratedRegex(@"^(.+?)(?:\s+\(.+?\))?\s+(.+?)$", RegexOptions.IgnoreCase)]
		private static partial Regex AgencyRegex();
#else
		static readonly Regex r_AgencyRegex = new(@"^(.+?)(?:\s+\(.+?\))?\s+(.+?)$", RegexOptions.IgnoreCase);
		static Regex AgencyRegex() => r_AgencyRegex;
#endif
		static void ExtractTitleAndSource(string agency, out string title, out string? source) {
			var m = AgencyRegex().Match(agency);
			if (!m.Success) {
				title = agency;
				source = null;
				return;
			}
			source = m.Groups[1].Value;
			title = m.Groups[2].Value;
		}

		sealed record ReportUnitKey(string Id) : IReportUnitKey { }
		sealed record ReportRevisionKey : IReportRevisionKey;
	}
}
