#nullable disable

using System.Xml.Serialization;

namespace Cryville.EEW.JMAAtom.Model.Seismology {
	/// <summary>
	/// 地震関連情報
	/// </summary>
	public class EarthquakeInfo {
		[XmlAttribute("type")]
		public string Type { get; set; }
		/// <summary>
		/// 情報名称
		/// </summary>
		/// <remarks>
		/// <para>情報の情報名称を表す。</para>
		/// </remarks>
		public string InfoKind { get; set; }
		/// <summary>
		/// 情報種別番号
		/// </summary>
		/// <remarks>
		/// <para>（南海トラフ地震に関連する情報）南海トラフ地震に関連する情報の情報種別（「南海トラフ地震臨時情報」においては情報名の後に付記するキーワードの種別、「南海トラフ地震関連解説情報」においては「南海トラフ沿いの地震に関する評価検討会」の定例会合における発表か否か）を表す番号を記入するために用いる。コード種別では @codeType として“地震関連情報番号コード”が用いられる。</para>
		/// </remarks>
		public InfoSerial InfoSerial { get; set; }
		/// <summary>
		/// 本文
		/// </summary>
		/// <remarks>
		/// <para>自由文形式により、情報の本文を記載する。</para>
		/// </remarks>
		public string Text { get; set; }
		/// <summary>
		/// 参考情報
		/// </summary>
		/// <remarks>
		/// <para>（南海トラフ地震に関連する情報）南海トラフ地震に関連する情報の種類などの参考情報を記載する。</para>
		/// <para>（北海道・三陸沖後発地震注意情報）北海道・三陸沖後発地震注意情報の概要や発表基準、留意事項などの参考情報を記載する。</para>
		/// </remarks>
		public string Appendix { get; set; }
	}
	/// <summary>
	/// 情報種別番号
	/// </summary>
	public class InfoSerial : NameCodePair {
		[XmlAttribute("codeType")]
		public string CodeType { get; set; }
	}
}