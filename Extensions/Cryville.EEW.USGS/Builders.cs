using Cryville.EEW.ComponentModel;
using Cryville.EEW.Report;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.USGS {
	public enum USGSWorkerSubtype {
		[LocalizableDisplayName("QuakeML", Path = ["WorkerSubtype"])] QuakeML,
		[LocalizableDisplayName("GeoJSON", Path = ["WorkerSubtype"])] GeoJSON,
	}
	[Export(typeof(IBuilder<ISourceWorker>))]
	public class USGSWorkerBuilder : IBuilder<USGSWorker> {
		public string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);

		[LocalizableDisplayName("PNSubtype")]
		public USGSWorkerSubtype Subtype { get; set; } = USGSWorkerSubtype.GeoJSON;

		public USGSWorker Build(ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return Subtype switch {
				USGSWorkerSubtype.GeoJSON => new USGSGeoJSONWorker(new Uri("https://earthquake.usgs.gov/earthquakes/feed/v1.0/geojson.php")),
				USGSWorkerSubtype.QuakeML => new USGSQuakeMLWorker(new Uri("https://earthquake.usgs.gov/earthquakes/feed/v1.0/quakeml.php")),
				_ => throw new ArgumentException(res.GetStringRequired("ErrorUnknownSubtype")),
			};
		}
	}

	[Export(typeof(IBuilder<IGenerator<ReportModel>>))]
	public class USGSContoursReportGeneratorBuilder : SimpleBuilder<USGSContoursReportGenerator> {
		public override string? GetName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("SourceNameContours");
		}
	}

	[Export(typeof(IBuilder<IGenerator<(global::QuakeML.Event, ReportModel), ReportModel>>))]
	public class USGSQuakeMLExtensionBuilder : SimpleBuilder<USGSQuakeMLExtension> {
		public override string? GetName([NotNull] ref CultureInfo? culture) => SharedResources.SourceName(ref culture);
	}
}
