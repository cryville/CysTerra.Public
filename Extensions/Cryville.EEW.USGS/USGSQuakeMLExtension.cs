using Cryville.EEW.Report;
using QuakeML;
using System.Globalization;
using System.Xml;

namespace Cryville.EEW.USGS {
	public class USGSQuakeMLExtension : IContextedGenerator<(Event, ReportModel), IReportGeneratorContext, ReportModel> {
		const string CATALOG_NAMESPACE = "http://anss.org/xmlns/catalog/0.1";
		public ReportModel Generate((Event, ReportModel) input, IReportGeneratorContext? context, ref CultureInfo culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;

			(var e, var result) = input;
			if (e.AnyAttr is XmlAttribute[] anyAttr) {
				string? dataID = null;
				string? source = null, id = null;
				bool hasCatalogAttr = false;
				foreach (var attr in anyAttr) {
					if (attr.NamespaceURI != CATALOG_NAMESPACE) continue;
					hasCatalogAttr = true;
					switch (attr.LocalName) {
						case "eventsource":
							source = attr.Value;
							break;
						case "eventid":
							id = attr.Value;
							break;
					}
				}
				if (source != null && id != null) {
#pragma warning disable IDE0079 // False report
#pragma warning disable CA1308
					string code = source.ToLowerInvariant() + id;
#pragma warning restore CA1308
#pragma warning restore IDE0079
					if (code != dataID) {
						result.GroupKeys.Add(new USGSEventIDGroupKey(code));
					}
				}
				if (hasCatalogAttr) {
					if (result.Source == null && source == null)
						result.Source = res.GetStringRequired("AuthorityName");
					else
						result.Source = string.Format(culture, res.GetStringRequired("AuthorityNameForwarded"), res.GetStringRequired("AuthorityName"), result.Source ?? source);
				}
			}
			return result;
		}
	}
}
