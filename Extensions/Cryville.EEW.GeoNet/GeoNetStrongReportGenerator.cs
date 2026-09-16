using Cryville.Common.Compat;
using Cryville.EEW.GeoNet.Model;
using Cryville.EEW.Report;
using System.Globalization;
using System.Linq;

namespace Cryville.EEW.GeoNet {
	public class GeoNetStrongReportGenerator : IContextedGenerator<GeoNetStrong, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(GeoNetStrong e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			var m = e.Metadata;
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("TitleStrong"),
				Source = res.GetStringRequired("AuthorityName"),
				TimeZone = Local.TimeZone,
			};

			if (!context.NameLocationTo(result, m.Latitude, m.Longitude, Local.Culture, culture)) {
				result.Location = m.Description;
				result.LocationSpecificity = 6;
			}

			result.GroupKeys.Add(new ReportUnitKey(m.EventID));
			result.GroupKeys.Add(new GeoNetPublicIDGroupKey(m.EventID));
			result.RevisionKey = new ReportRevisionKey();

			int maxMMI = e.Features.Max(f => f.Properties.MMI);
			result.Properties.Add(new(TagTypeKeys.IntensityMMI, res.GetStringRequired("PropertyMaxIntensity"), RomanNumerals.ToRomanNumeralChar(maxMMI, culture), context.SeverityScheme, maxMMI) { AccuracyOrder = 10 });
			result.Properties.Add(new(TagTypeKeys.Magnitude, m.MagnitudeType, m.Magnitude.ToString("F1", culture), context.SeverityScheme, m.Magnitude) { AccuracyOrder = 10 });
			result.Properties.Add(new(TagTypeKeys.HypocenterDepth, res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), m.Depth.ToString("F1", culture)), context.SeverityScheme, m.Depth) { AccuracyOrder = 10 });

			return result;
		}
		sealed record ReportUnitKey(string PublicID) : IReportUnitKey;
		sealed record ReportRevisionKey : IReportRevisionKey;
	}
}
