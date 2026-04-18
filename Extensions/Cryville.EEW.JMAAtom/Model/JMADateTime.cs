using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	public class JMADateTime {
		public DateTimeOffset Value { get; set; }
		[XmlText]
		public string RawValue {
			get => Value.ToString("O", CultureInfo.InvariantCulture);
			set => Value = XmlConvert.ToDateTimeOffset(value);
		}
		[XmlAttribute("type")]
		public string Type { get; set; }
		[XmlAttribute("significant")]
		public string Significant { get; set; }
		[XmlAttribute("precision")]
		public string Precision { get; set; }
		[XmlAttribute("dubious")]
		public string Dubious { get; set; }
		[XmlAttribute("description")]
		public string Description { get; set; }
	}
}
