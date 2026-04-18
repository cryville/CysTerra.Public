using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	public class MeasuredValue<T> : TypedValue<T> {
		[XmlAttribute("unit")]
		public string Unit { get; set; }
		[XmlAttribute("condition")]
		public string Condition { get; set; }
		[XmlAttribute("description")]
		public string Description { get; set; }
	}
}
