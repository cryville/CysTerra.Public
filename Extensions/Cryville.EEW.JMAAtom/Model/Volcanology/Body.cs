using System.Collections.ObjectModel;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model.Volcanology {
	public class Body {
		public string Notice { get; set; }
		[XmlElement("VolcanoInfo")]
		public Collection<VolcanoInfo> VolcanoInfo { get; set; }
		public AshInfos AshInfos { get; set; }
		public VolcanoInfoContent VolcanoInfoContent { get; set; }
		public VolcanoObservation VolcanoObservation { get; set; }
		public string Text { get; set; }
	}

	public class VolcanoInfo {
		[XmlAttribute("type")]
		public string Type { get; set; }
		[XmlElement("Item")]
		public Collection<Item> Items { get; set; }
	}

	public class Item {
		public EventTime EventTime { get; set; }
		public Kind Kind { get; set; }
		public Kind LastKind { get; set; }
		public Areas Areas { get; set; }
	}

	public class EventTime {
		public JMADateTime EventDateTime { get; set; }
		public JMADateTime EventDateTimeUTC { get; set; }
		public string EventDateTimeComment { get; set; }
	}

	public class Kind : NameCodePair {
		public string FormalName { get; set; }
		public string Condition { get; set; }
		public VolcanoProperty Property { get; set; }
	}

	public class Areas {
		[XmlAttribute("codeType")]
		public string CodeType { get; set; }
		[XmlElement("Area")]
		public Collection<Area> Items { get; set; }
	}

	public class Area : NameCodePair {
		public Coordinate Coordinate { get; set; }
		public string AreaFromMark { get; set; }
		public string CraterName { get; set; }
		public Coordinate CraterCoordinate { get; set; }
	}

	public class AshInfos {
		[XmlAttribute("type")]
		public string Type { get; set; }
		[XmlElement("AshInfo")]
		public Collection<AshInfo> Items { get; set; }
	}

	public class AshInfo {
		[XmlAttribute("type")]
		public string Type { get; set; }
		public XmlSerializedDateTimeOffset StartTime { get; set; }
		public XmlSerializedDateTimeOffset EndTime { get; set; }
		[XmlElement("Item")]
		public Collection<Item> Items { get; set; }
	}

	public class VolcanoProperty {
		public Size Size { get; set; }
		[XmlElement("Polygon", Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public Collection<Polygon> Polygons { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<string> PlumeDirection { get; set; }
		public Distance Distance { get; set; }
		public string Remark { get; set; }
	}

	public class Size : TypedValue<float> {
		[XmlAttribute("unit")]
		public string Unit { get; set; }
	}

	public class Distance : TypedValue<string> {
		[XmlAttribute("unit")]
		public string Unit { get; set; }
		[XmlAttribute("description")]
		public string Description { get; set; }
	}

	public class VolcanoInfoContent {
	}

	public class VolcanoObservation {
		public EventTime EventTime { get; set; }
		public Plume ColorPlume { get; set; }
		public Plume WhitePlume { get; set; }
		public WindAboveCrater WindAboveCrater { get; set; }
		public string OtherObservation { get; set; }
		public string Appendix { get; set; }
	}

	public class Plume {
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<int> PlumeHeightAboveCrater { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<int> PlumeHeightAboveSeaLevel { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<string> PlumeDirection { get; set; }
		public string PlumeComment { get; set; }
	}

	public class WindAboveCrater {
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public JMADateTime DateTime { get; set; }
		[XmlElement("WindAboveCraterElements")]
		public Collection<WindAboveCraterElements> WindAboveCraterElements { get; set; }
	}

	public class WindAboveCraterElements {
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<int> WindHeightAboveSeaLevel { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public ReferencedValue<float> WindDegree { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public ReferencedValue<float> WindSpeed { get; set; }
		[XmlAttribute("heightProperty")]
		public string HeightProperty { get; set; }
		[XmlAttribute("description")]
		public string Description { get; set; }
	}
}
