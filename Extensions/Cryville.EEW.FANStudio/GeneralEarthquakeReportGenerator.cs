using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Cryville.EEW.FANStudio {
	public abstract class GeneralEarthquakeReportGenerator {
		protected abstract string Source { get; }
		protected virtual CultureInfo LocalCulture => Local.UnitedStatesCulture;
		protected virtual string ProcessLocalLocationName(string name) => name;

		public ReportModel Generate(GeneralEarthquake e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(Source, ref culture);
			var res = lres.RootMessageStringSet;
			var utcTime = TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, Local.TimeZone);
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Location = ProcessLocalLocationName(e.PlaceName),
				LocationSpecificity = 3,
				Time = new(utcTime, TimeSpan.Zero),
				TimeZone = TimeZoneInfo.Utc,
			};

			result.GroupKeys.Add(new ReportUnitKey(Source, e.ID));
			result.RevisionKey = new ReportRevisionKey();
			context.NameLocationTo(result, e.Latitude, e.Longitude, LocalCulture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, utcTime, e.Magnitude ?? 0, e.Depth));

			if (e.Magnitude is float mag) result.Properties.Add(new("Magnitude", res.GetStringRequired("PropertyMagnitude"), mag.ToString("F1", culture), context.SeverityScheme, mag) { AccuracyOrder = 10 });
			if (e.Depth is float depth) result.Properties.Add(new("HypocenterDepth", res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth.ToString("F0", culture)), context.SeverityScheme, depth) { AccuracyOrder = 10 });
			return result;
		}

		sealed record ReportUnitKey(string Source, string ID) : IReportUnitKey { }
		sealed record ReportRevisionKey : IReportRevisionKey;
	}

	public class EMSCEarthquakeReportGenerator : GeneralEarthquakeReportGenerator, IContextedGenerator<EMSCEarthquake, IReportGeneratorContext, ReportModel> {
		protected override string Source => nameof(EMSCEarthquake);
		protected override CultureInfo LocalCulture => Local.GreatBritainCulture;
		public ReportModel Generate(EMSCEarthquake e, IReportGeneratorContext? context, ref CultureInfo culture) => base.Generate(e, context, ref culture);
	}
	public partial class BCSFEarthquakeReportGenerator : GeneralEarthquakeReportGenerator, IContextedGenerator<BCSFEarthquake, IReportGeneratorContext, ReportModel> {
		protected override string Source => nameof(BCSFEarthquake);
#if NET7_0_OR_GREATER
		[GeneratedRegex(@"^.*? of magnitude [0-9\.]+,\s*")]
		private static partial Regex MagnitudePrefixRegex();
#else
		static readonly Regex r_MagnitudePrefixRegex = new(@"^.*? of magnitude [0-9\.]+,\s*");
		static Regex MagnitudePrefixRegex() => r_MagnitudePrefixRegex;
#endif
		protected override string ProcessLocalLocationName(string name) => MagnitudePrefixRegex().Replace(name, "");
		public ReportModel Generate(BCSFEarthquake e, IReportGeneratorContext? context, ref CultureInfo culture) => base.Generate(e, context, ref culture);
	}
	public class GFZEarthquakeReportGenerator : GeneralEarthquakeReportGenerator, IContextedGenerator<GFZEarthquake, IReportGeneratorContext, ReportModel> {
		protected override string Source => nameof(GFZEarthquake);
		public ReportModel Generate(GFZEarthquake e, IReportGeneratorContext? context, ref CultureInfo culture) => base.Generate(e, context, ref culture);
	}
	public class USPEarthquakeReportGenerator : GeneralEarthquakeReportGenerator, IContextedGenerator<USPEarthquake, IReportGeneratorContext, ReportModel> {
		protected override string Source => nameof(USPEarthquake);
		public ReportModel Generate(USPEarthquake e, IReportGeneratorContext? context, ref CultureInfo culture) => base.Generate(e, context, ref culture);
	}
}
