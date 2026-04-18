using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	public class TypedValue<T> {
		[XmlText]
		public T Value { get; set; }
		[XmlAttribute("type")]
		public string Type { get; set; }
	}
}
