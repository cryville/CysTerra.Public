using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	[XmlRoot("Report", Namespace = "http://xml.kishou.go.jp/jmaxml1/")]
	public class JMAReport {
		public Control Control { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/informationBasis1/")]
		public Head Head { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/body/seismology1/", Type = typeof(Seismology.Body))]
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/body/volcanology1/", Type = typeof(Volcanology.Body))]
		public object Body { get; set; }

		public override string ToString() => $"JMAReport {{ Title = {Control.Title}, EditorialOffice = {Control.EditorialOffice}, Status = {Control.Status}, EventID = {Head.EventID}, DateTime = {Control.DateTime} }}";
	}
}
