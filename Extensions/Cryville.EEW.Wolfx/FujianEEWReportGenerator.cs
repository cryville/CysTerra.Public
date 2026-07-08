using Cryville.Common.Compat;
using Cryville.EEW.Report;
using Cryville.EEW.Wolfx.Model;
using System;
using System.Globalization;

namespace Cryville.EEW.Wolfx {
	public sealed class FujianEEWReportGenerator : IContextedGenerator<FujianEEW, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(FujianEEW e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource(nameof(FujianEEW), ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
				Source = res.GetStringRequired("AuthorityName"),
				Time = new(e.OriginTime, Local.TimeZoneOffset),
				InvalidatedTime = new DateTimeOffset(e.ReportTime, Local.TimeZoneOffset) + TimeSpan.FromMinutes(5),
				TimeZone = Local.TimeZone,
			};
			if (WolfxHelpers.ExtractEventID(e.EventID) is string id)
				result.GroupKeys.Add(new ReportUnitKey(id));
			if (!context.NameLocationTo(result, e.Latitude, e.Longitude, Local.Culture, culture)) {
				result.Location = e.HypoCenter;
				result.LocationSpecificity = 6;
			}
			result.RevisionKey = new ReportRevisionKey(e.ReportNum, e.IsFinal);
			result.Properties.Add(new(TagTypeKeys.Magnitude, res.GetStringRequired("PropertyMagnitude"), e.Magnitude.ToString("F1", culture), context.SeverityScheme, e.Magnitude) { AccuracyOrder = 70 });
			result.GroupKeys.Add(new HypocenterGroupKey(e.Latitude, e.Longitude, TimeZoneInfo.ConvertTimeToUtc(e.OriginTime, result.TimeZone), e.Magnitude));
			return result;
		}

		sealed record ReportUnitKey(string EventID) : IReportUnitKey { }
		sealed record ReportRevisionKey(int ReportNum, bool IsFinal = false) : IReportRevisionKey {
			public int? Serial => ReportNum;
			public bool IsFinalRevision => IsFinal;
		}
	}
}
