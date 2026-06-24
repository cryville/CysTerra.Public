using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Report;
using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public sealed class ShanxiEarthquakeReportGenerator : IContextedGenerator<ShanxiEarthquake, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(ShanxiEarthquake? e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(ShanxiEarthquake), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.ShockTime, Local.TimeZoneOffset),
				TimeZone = Local.TimeZone,
				Model = e,
			};
			context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture);
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.ShockTime, result.TimeZone), e.Magnitude, e.Depth));
			result.Properties.Add(new("Magnitude:SurfaceWave", res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 10 });

			if (result.Location == null) {
				result.Location = e.PlaceName;
				result.LocationSpecificity = 5;
			}

			result.Properties.Add(new("HypocenterDepth", res.GetStringRequired("PropertyDepth"), string.Format(culture, res.GetStringRequired("PropertyDepthValue"), e.Depth), context.SeverityScheme, e.Depth) { AccuracyOrder = 10 });

			return result;
		}
	}
}
