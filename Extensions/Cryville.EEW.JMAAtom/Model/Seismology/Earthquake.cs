using System.Collections.ObjectModel;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model.Seismology {
	public class Earthquake {
		public XmlSerializedDateTimeOffset OriginTime { get; set; }
		public XmlSerializedDateTimeOffset ArrivalTime { get; set; }
		public string Condition { get; set; }
		public Hypocenter Hypocenter { get; set; }
		[XmlElement("Magnitude", Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public Collection<Magnitude> Magnitudes { get; set; }
	}

	public class Hypocenter {
		public HypoArea Area { get; set; }
		public string Source { get; set; }
		public Accuracy Accuracy { get; set; }
	}

	public class HypoArea {
		public string Name { get; set; }
		public TypedValue<string> Code { get; set; }
		[XmlElement("Coordinate", Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public Collection<Coordinate> Coordinates { get; set; }
		public string ReduceName { get; set; }
		public TypedValue<string> ReduceCode { get; set; }
		public string DetailedName { get; set; }
		public TypedValue<string> DetailedCode { get; set; }
		public string NameFromMark { get; set; }
		public TypedValue<string> MarkCode { get; set; }
		public string Direction { get; set; }
		public HypoAreaDistance Distance { get; set; }
		public string LandOrSea { get; set; }

		public override string ToString() => $"{Name} ({string.Join(", ", Coordinates)})";
	}

	public class HypoAreaDistance {
		[XmlText]
		public int Value { get; set; }
		[XmlAttribute("unit")]
		public string Unit { get; set; }
	}

	public class Accuracy {
		public AccuracyEpicenter Epicenter { get; set; }
		public AccuracyValue Depth { get; set; }
		public AccuracyValue MagnitudeCalculation { get; set; }
		public int NumberOfMagnitudeCalculation { get; set; }
	}

	public class AccuracyEpicenter : AccuracyValue {
		[XmlAttribute("rank2")]
		public int Rank2 { get; set; }
	}

	public class AccuracyValue {
		[XmlText]
		public float Value { get; set; }
		[XmlAttribute("rank")]
		public int Rank { get; set; }
	}
}
