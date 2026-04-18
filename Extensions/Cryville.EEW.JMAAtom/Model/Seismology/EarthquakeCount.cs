using System.Collections.ObjectModel;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model.Seismology {
	public class EarthquakeCount {
		/// <summary>
		/// 地震回数（期間別）
		/// </summary>
		/// <remarks>
		/// <para>特定の時間幅で区切った期間内での地震回数を記載する。期間を複数に分けて発表する場合は、本要素が複数出現する。設定する期間幅に応じて、@type に“１時間地震回数”（1 時間単位）、“累積地震回数”（全期間の合計）、“地震回数”（その他の場合）を記載し、具体的な期間を子要素 StartTime 及び子要素 EndTime で指定する。</para>
		/// <para>無感地震を含む全ての地震回数を発表する場合は子要素 Number に、有感地震回数に限定して発表する場合は、子要素 FeltNumber に値を記載する。値を発表しない要素には“-1”を記載する。</para>
		/// </remarks>
		[XmlElement("Item")]
		public Collection<CountData> Items { get; set; }
	}

	public class CountData {
		[XmlAttribute("type")]
		public string Type { get; set; }
		public XmlSerializedDateTimeOffset StartTime { get; set; }
		public XmlSerializedDateTimeOffset EndTime { get; set; }
		public int Number { get; set; }
		public int FeltNumber { get; set; }
		public string Condition { get; set; }
	}
}
