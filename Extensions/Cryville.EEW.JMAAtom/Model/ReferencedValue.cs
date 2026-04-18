using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	public class ReferencedValue<T> : MeasuredValue<T> {
		[XmlAttribute("refID")]
		public byte RefId { get; set; }
	}
}
