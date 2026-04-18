using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	/// <summary>
	/// ヘッダ部
	/// </summary>
	/// <remarks>
	/// <para>本情報の見出しを記載する。</para>
	/// </remarks>
	public class Head {
		/// <summary>
		/// 標題
		/// </summary>
		/// <remarks>
		/// <para>情報の標題を記載する。</para>
		/// <para>震源・震度に関する情報において、近地地震の場合には“震源・震度情報”、遠地地震の場合には“遠地地震に関する情報”と記載する。</para>
		/// <para>津波警報・注意報・予報については、発表する情報に含まれる津波予報等の種類の総和表現を記載する。なお、津波警報・注意報を全解除し、全ての津波予報区等で津波予報（若干の海面変動）又は津波なしとなる場合は、事例に示すとおり“津波予報”と記載する。</para>
		/// <para>各地の満潮時刻と津波到達予想時刻を発表する津波情報については“各地の満潮時刻・津波到達予想時刻に関する情報”を、津波の観測値を発表する津波情報については“津波観測に関する情報”を記載する。両者をひとつの津波情報電文で発表する場合は、本要素の中に二つの標題を半角スペースで区切って併記する。</para>
		/// <para>南海トラフ地震に関連する情報においては、情報名称（Control/Title）が”南海トラフ地震臨時情報”の場合は、”南海トラフ地震臨時情報”に続けて情報種別番号名（Body/EarthquakeInfo/InfoSerial/Name）の内容を付記する（例：”南海トラフ地震臨時情報（巨大地震警戒）”）。また、情報名称（Control/Title）が”南海トラフ地震関連解説情報”の場合は、”南海トラフ地震関連解説情報”と標記し、情報番号（Head/Serial）に値が記載されている場合に限り、一連の情報番号を付記する（例：”南海トラフ地震関連解説情報（第○号）”）。</para>
		/// <para>火山に関連する情報においては、火山名と情報の種別を記載する。</para>
		/// </remarks>
		public string Title { get; set; }
		/// <summary>
		/// 発表時刻
		/// </summary>
		/// <remarks>
		/// <para>発表官署が本情報を発表した時刻を記載する。</para>
		/// <para>緊急地震速報（警報）、緊急地震速報（地震動予報）、緊急地震速報（予報）、及び緊急地震速報の配信テスト電文については秒値まで、その他の地震・津波・南海トラフ地震・火山に関連する情報については、分値まで有効である。</para>
		/// </remarks>
		public XmlSerializedDateTimeOffset ReportDateTime { get; set; }
		/// <summary>
		/// 基点時刻
		/// </summary>
		/// <remarks>
		/// <para>情報の内容が発現・発効する基点時刻を記載する。</para>
		/// <para>震度速報については最初に地震波を自動検知した観測点における地震波の検知時刻を、地震情報（顕著な地震の震源要素更新のお知らせ）については震源要素を切り替えた時刻を、津波の観測値を発表する津波情報、沖合の津波観測に関する情報については津波の観測状況を確定した時刻を記載する。火山現象に関する海上警報については火山活動の観測時刻、噴火に関する火山観測報、噴火速報、推定噴煙流向報については報じる現象の発現時刻、降灰予報については情報の対象となる時間帯の基点時刻を記載する。その他の地震・津波・火山に関連する情報については、ヘッダ部の発表時刻（Head/ReportDateTime）の値を記載する。</para>
		/// <para>なお、緊急地震速報（警報）、緊急地震速報（地震動予報）、緊急地震速報（予報）、及び緊急地震速報の配信テスト電文については秒値まで、その他の地震・津波・南海トラフ地震・火山に関連する情報については、分値まで有効である。ただし、噴火に関する火山観測報、噴火速報、推定噴煙流向報については、基本的に分値まで有効であるが、TargetDTDubious が出現する場合は、それで示すあいまいさに応じた単位までが有効、発現時刻が不明の場合には xsi:nil=“true”属性値により空要素となる。</para>
		/// </remarks>
		public XmlSerializedDateTimeOffset TargetDateTime { get; set; }
		/// <summary>
		/// 基点時刻のあいまいさ
		/// </summary>
		public string TargetDTDubious { get; set; }
		/// <summary>
		/// 基点時刻からの取りうる時間
		/// </summary>
		public TimeSpan TargetDuration { get; set; }
		/// <summary>
		/// 失効時刻
		/// </summary>
		public XmlSerializedDateTimeOffset ValidDateTime { get; set; }
		/// <summary>
		/// 識別情報
		/// </summary>
		/// <remarks>
		/// <para>地震・津波に関連する情報については、ある特定の地震を識別するための地震識別番号（14 桁の数字）を記載する。津波に関連する情報では、当該警報等に寄与している地震の地震識別番号を記載するため、１つの電文に複数の地震識別番号が出現する場合もある。詳細については、（ⅲ）共通別紙イ．「地震・津波に関連する情報の EventID 要素の運用」を参照。</para>
		/// <para>南海トラフ地震に関連する情報については、任意の識別番号（14 桁の数字）を記載する。詳細については、（ⅲ）共通別紙エ．「南海トラフ地震に関連する情報における EventID 要素及び Serial 要素の運用」を参照。</para>
		/// <para>火山に関連する情報については、３桁の火山番号を記載する。ただし、噴火に関する火山観測報及び噴火速報、推定噴煙流向報については、ReportDateTime と火山番号を“_”で連結して記載する。</para>
		/// <para>地震・津波に関するお知らせや火山に関するお知らせについては、情報発表日時分（14 桁の数字）を記載する。</para>
		/// </remarks>
		public string EventID { get; set; }
		/// <summary>
		/// 情報形態
		/// </summary>
		/// <remarks>
		/// <para>情報を発表する場合は“発表”を、「独立した情報単位」において直前の時点で発表されている Control/DateTime の最も新しい電文を訂正する場合は“訂正”を、「独立した情報単位」全体を取り消す場合は“取消”を記載する。取消電文の運用については、（ⅲ）共通別紙ウ．「取消電文の運用」を参照。</para>
		/// </remarks>
		public string InfoType { get; set; }
		/// <summary>
		/// 情報番号
		/// </summary>
		/// <remarks>
		/// <para>続報を発表し、内容を更新する情報については、情報番号を記載する。続報を発表する度に情報番号を更新するが、取消報の場合は、番号は更新しない。訂正報の場合は訂正する直近の情報の情報番号を記載する。</para>
		/// <para>南海トラフ地震に関連する情報については、続報を発表する情報で情報番号を記載する。詳細については、（ⅲ）共通別紙エ．「南海トラフ地震に関連する情報における EventID 要素及び Serial 要素の運用」を参照。</para>
		/// <para>※なお、同一種別の情報における最新情報の検索にあたっては、本要素ではなく管理部の発表時刻（Control/DateTime）を参照すること。</para>
		/// </remarks>
		public string Serial { get; set; }
		/// <summary>
		/// スキーマの運用種別情報（「気象警報･注意報」、「津波警報･注意報」など）
		/// </summary>
		public string InfoKind { get; set; }
		/// <summary>
		/// スキーマの運用種別情報のバージョン
		/// </summary>
		public string InfoKindVersion { get; set; }
		/// <summary>
		/// 見出し要素
		/// </summary>
		/// <remarks>
		/// <para>子要素に Text 及び Information をもつ。</para>
		/// </remarks>
		public JMAHeadline Headline { get; set; }

		public override string ToString() => $"{Title}\n\n{Headline}";
	}

	/// <summary>
	/// 見出し要素
	/// </summary>
	public class JMAHeadline {
		/// <summary>
		/// 見出し文
		/// </summary>
		/// <remarks>
		/// <para>見出し文を自由文形式で記載する。</para>
		/// </remarks>
		public string Text { get; set; }
		/// <summary>
		/// 見出し防災気象情報事項
		/// </summary>
		/// <remarks>
		/// <para>地震火山関連 XML 電文では、情報によって本要素の運用が異なる。このため、以下のとおり個別に解説する。</para>
		/// <list type="bullet">
		/// <item>津波に関連する情報については、11-2(1)にて解説する。</item>
		/// <item>緊急地震速報については、11-2(2)にて解説する。</item>
		/// <item>地震情報等については、11-2(3)にて解説する。</item>
		/// <item>南海トラフ地震に関連する情報では、本要素は出現しない。</item>
		/// <item>地震・津波に関するお知らせでは、本要素は出現しない。</item>
		/// <item>火山に関連する情報については、11-2(4)にて解説する。</item>
		/// <item>火山に関するお知らせでは、本要素は出現しない。</item>
		/// </list>
		/// <para>なお、情報形態（Head/InfoType）が“取消”の場合、情報名称に関わらず本要素は出現しない（（ⅲ）共通別紙ウ．「取消電文の運用」を参照）。</para>
		/// </remarks>
		[XmlElement]
		public Collection<JMAHeadlineInformation> Information { get; set; }

		public override string ToString() => $"{Text}\n\n{(Information == null ? "" : string.Join("\n\n", Information))}";
	}

	/// <summary>
	/// 見出し防災気象情報事項
	/// </summary>
	public class JMAHeadlineInformation {
		[XmlAttribute("type")]
		public string Type { get; set; }
		/// <summary>
		/// 個々の防災気象情報要素（要素、直前の状況、対象地域・地点全体）
		/// </summary>
		[XmlElement("Item")]
		public Collection<JMAHeadlineInformationItem> Items { get; set; }

		public override string ToString() => $"{Type}\n{string.Join('\n', Items)}";
	}

	/// <summary>
	/// 防災気象情報要素（要素、直前の状況、対象地域・地点全体）
	/// </summary>
	public class JMAHeadlineInformationItem {
		[XmlElement("Kind")]
		public Collection<Kind> Kinds { get; set; }
		[XmlElement("LastKind")]
		public Collection<Kind> LastKinds { get; set; }
		public Areas Areas { get; set; }

		public override string ToString() => $"{string.Join(", ", Kinds)}: {Areas}";
	}

	public class Kind : NameCodePair {
		public string Condition { get; set; }
	}

	public class Areas {
		[XmlAttribute("codeType")]
		public string CodeType { get; set; }
		[XmlElement("Area")]
		public Collection<Area> Values { get; set; }

		public override string ToString() => string.Join(", ", Values);
	}

	public class Area : NameCodePair {
		// TODO Geometries
	}
}
