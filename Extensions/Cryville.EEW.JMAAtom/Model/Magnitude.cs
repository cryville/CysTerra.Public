using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	public class Magnitude : TypedValue<float> {
		[XmlAttribute("condition")]
		public string Condition { get; set; }
		[XmlAttribute("description")]
		public string Description { get; set; }

		public override string ToString() => $"{Type}{Value}";
	}
}
