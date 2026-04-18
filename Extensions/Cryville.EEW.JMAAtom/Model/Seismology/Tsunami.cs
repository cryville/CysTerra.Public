using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model.Seismology {
	/// <summary>
	/// 津波
	/// </summary>
	public class Tsunami {
		public string Release { get; set; }
		/// <summary>
		/// 津波の観測値
		/// </summary>
		/// <remarks>
		/// <para>津波が観測された場合、本要素に津波の観測に関する情報を記載する。</para>
		/// </remarks>
		public TsunamiDetail Observation { get; set; }
		/// <summary>
		/// 津波の推定値
		/// </summary>
		/// <remarks>
		/// <para>沖合の潮位観測点で観測された津波の情報に基づき、津波が到達すると推定される沿岸地域について、津波の推定値に関する情報を記載する。</para>
		/// </remarks>
		public TsunamiDetail Estimation { get; set; }
		/// <summary>
		/// 津波の予測値
		/// </summary>
		/// <remarks>
		/// <para>津波警報・注意報・予報に関する情報を本要素に記載する。</para>
		/// </remarks>
		public TsunamiDetail Forecast { get; set; }
	}

	/// <summary>
	/// 津波情報
	/// </summary>
	public class TsunamiDetail {
		/// <summary>
		/// コード体系の定義
		/// </summary>
		public CodeDefine CodeDefine { get; set; }
		/// <summary>
		/// 津波の予測値（津波予報区毎）
		/// </summary>
		/// <remarks>
		/// <para>（予測値）本情報で津波警報・注意報や津波予報（若干の海面変動）を発表している津波予報区及び津波警報・注意報を解除した津波予報区について、発表状況を記載する。記載する津波予報区の数に応じて、本要素が複数出現する。</para>
		/// <para>（観測値）津波予報区毎に津波の観測値を記載する。津波を観測した津波予報区の数に応じて、本要素が複数出現する。</para>
		/// <para>（推定値）沿岸地域毎に推定される津波の到達時刻、高さ等の情報を記載する。推定値を発表する沿岸地域の数に応じて、本要素が複数出現する。</para>
		/// </remarks>
		[XmlElement("Item")]
		public Collection<TsunamiItem> Items { get; set; }
	}

	public class TsunamiItem {
		/// <summary>
		/// 津波予報区
		/// </summary>
		/// <remarks>
		/// <para>対象となる津波予報区の名称を子要素 Name に、対応するコードを子要素 Code に記載する。対応するコードは、「コード体系の定義」（Body/Tsunami/Forecast/CodeDefine）で定義されている。具体的なコードの値については、別途提供するコード表を参照。</para>
		/// <para>（観測値・沖合）沖合の潮位観測点は津波予報区に所属していないため、本情報においては子要素 Nameおよび Code は常に空要素となる。</para>
		/// </remarks>
		public ForecastArea Area { get; set; }
		/// <summary>
		/// 津波警報等の種類
		/// </summary>
		/// <remarks>
		/// <para>本情報による、当該津波予報区の津波警報等の発表状況を子要素Kind に記載する。また、発表状況の状態遷移を表すために、一つ前の情報による発表状況を子要素 LastKind に記載する。さらに、各要素の子要素 Name 及び Code に、その名称と対応するコードを記載する。対応するコードは、「コード体系の定義」（Body/Tsunami/Forecast/CodeDefine）で定義されている。具体的なコードの値については、別途提供するコード表を参照。</para>
		/// <para>大津波警報については、第 1 報で大津波警報となる津波予報区および切り替え（更新報）で新たに大津波警報となる津波予報区においては”大津波警報：発表”、大津波警報を継続する津波予報区においては”大津波警報”を記載する。</para>
		/// </remarks>
		public Category Category { get; set; }
		/// <summary>
		/// <para>（予測値）津波の到達予想時刻（津波予報区）</para>
		/// <para>（推定値）津波到達時刻（推定値）</para>
		/// </summary>
		/// <remarks>
		/// <para>（予測値）当該津波予報区への第１波の到達予想時刻を、子要素 ArrivalTime に記載する。</para>
		/// <para>（予測値）本情報の発表時点において、第１波の到達予想時刻までに時間的な猶予が無い場合は、子要素 Condition を追加し、“ただちに津波来襲と予測”を記載する。また、既に第１波が到達したと推測される場合、当該津波予報区内の潮位観測点で第１波が観測された場合は、ArrivalTime に代わって子要素 Condition が出現し、それぞれ、“津波到達中と推測”、“第１波の到達を確認”を記載する。</para>
		/// <para>（推定値）当該沿岸地域に第１波が到達すると推定される時刻を子要素 ArrivalTime に記載する。</para>
		/// <para>（推定値）沖合の潮位観測点による観測値から当該沿岸地域への津波到達予想時刻を推定し、推定時刻よりも早く沿岸地域に津波が到達している可能性がある場合は、子要素 Condition を追加し、“早いところでは既に津波到達と推定”と記載する。</para>
		/// <para>続報において、新たに本要素が出現する場合は子要素 Revise に“追加”を、既出であった本要素の内容が更新される場合は“更新”を記載する。</para>
		/// <para>（予測値）また、津波警報・注意報を解除する又は津波予報（若干の海面変動）を発表している津波予報区については、本要素は出現しない。</para>
		/// </remarks>
		public FirstHeight FirstHeight { get; set; }
		/// <summary>
		/// <para>（予測値）予想される津波の高さ（津波予報区）</para>
		/// <para>（推定値）津波の高さ（推定値）</para>
		/// </summary>
		/// <remarks>
		/// <para>（予測値）当該津波予報区に対して予想される津波の高さを子要素 jmx_eb:TsunamiHeight にメートル単位で記載する。jmx_eb:TsunamiHeight の@type に“津波の高さ”を、@unit に“m”を記載する。また、@description に文字列表現を記載する。発表する津波の高さのとりうる値を下表に示す。jmx_eb:TsunamiHeight に記載する値は xs:float 型とし、「～未満」又は「～超」の表現は、事例に示すとおり@description に記載する。</para>
		/// <para>（推定値）沖合の潮位観測点によるこれまでの最大波の観測値から、当該沿岸地域に到達すると推定される時刻を子要素 DateTime に、津波の高さを子要素 jmx_eb:TsunamiHeight に記載する。子要素 jmx_eb:TsunamiHeight の@type に“津波の高さ”、@unit に津波の高さの単位である“m”、@description に文字列表現を記載する。発表する津波の高さのとりうる値を下表に示す。jmx_eb:TsunamiHeight に記載する値は xs:float 型とし、「～超」の表現は、事例に示すとおり @description に記載する。</para>
		/// <para>マグニチュードが 8 を超える巨大地震と推定されるなど、地震規模推定の不確定性が大きい場合は、これらの属性に加えて @condition が出現し、ここに津波の高さが不明である旨を示す固定値“不明”を記載する。津波の高さの値には“NaN”を記載する。また、@description に津波の高さに関する定性的表現を記載する。発表する定性的表現のとりうる値を下表に示す。定性的表現がない津波注意報や津波予報の場合は、@description は空属性となる。</para>
		/// <para>（予測値）大津波警報の津波予報区に対して、予想される津波の高さが最初に数値で発表された場合や、大津波警報の中で予想される津波の高さが上方修正された場合は、子要素 Condition を追加し、ここに”重要”と記載する。</para>
		/// <para>（推定値）津波警報以上の沿岸地域に対して推定される津波の高さが、予想される高さに比べて十分小さい場合は、子要素 DateTime 及び子要素 jmx_eb:TsunamiHeight に代わって子要素 Condition が出現し、ここに“推定中”と記載する（予想される高さが定性的表現で発表されている場合を除く）。</para>
		/// <para>（推定値）推定される津波の高さが大津波警報の基準を超え、追加あるいは更新された場合（定性的表現から数値表現に変更された場合も含む）は、子要素 Condition を追加し、ここに“重要”と記載する。</para>
		/// <para>続報において、新たに本要素が出現する場合は子要素 Revise に“追加”を、既出であった本要素の内容が更新される場合は“更新”を記載する。</para>
		/// <para>（予測値）また、津波が減衰して、いずれかの津波予報区で津波警報・注意報等の種類を引き下げる場合（解除、津波予報（若干の海面変動）への切り替えを含む）は、津波警報・注意報を解除した又は津波予報（若干の海面変動）を発表している全ての津波予報区について本要素は出現しない。</para>
		/// </remarks>
		public MaxHeight MaxHeight { get; set; }
		public TimeSpan Duration { get; set; }
		/// <summary>
		/// 潮位観測点
		/// </summary>
		/// <remarks>
		/// <para>（観測値）潮位観測点毎に津波の観測値を記載する。津波を観測した潮位観測点の数に応じて、本要素が複数出現する。</para>
		/// <para>（観測値）潮位観測点の名称を子要素 Name に、対応するコードを子要素 Code に記載する。対応するコードは、「コード体系の定義」（Body/Tsunami/Observation/CodeDefine）で定義されている。具体的なコードの値については、別途提供するコード表を参照。</para>
		/// <para>（予測値）対象となる潮位観測点の名称を子要素 Name に、対応するコードを子要素 Code に記載する。対応するコードは、「コード体系の定義」（Body/Tsunami/Forecast/CodeDefine）で定義されている。具体的なコードの値については、別途提供するコード表を参照。</para>
		/// <para>（予測値）また、当該観測点での満潮時刻を子要素 HighTideDateTime に、津波の到達予想時刻を子要素 FirstHeight に記載する。津波警報・注意報を解除した又は津波予報（若干の海面変動）を発表している津波予報区について、本要素は出現しない。</para>
		/// <para>（観測値・沖合）特殊観測機器の名称を子要素 Sensor に記載する。</para>
		/// </remarks>
		[XmlElement("Station")]
		public Collection<TsunamiStation> Stations { get; set; }
	}

	public class ForecastArea : NameCodePair {
		[XmlElement("City")]
		public Collection<NameCodePair> Cities { get; set; }
	}

	public class TsunamiStation : NameCodePair {
		/// <summary>
		/// （観測値・沖合）特殊観測機器の名称
		/// </summary>
		public string Sensor { get; set; }
		public XmlSerializedDateTimeOffset HighTideDateTime { get; set; }
		/// <summary>
		/// <para>（観測値）津波の第１波（観測値）</para>
		/// <para>（予測値）津波の到達予想時刻（潮位観測点）</para>
		/// </summary>
		/// <remarks>
		/// <para>（観測値）観測した津波の第１波について、子要素 ArrivalTime に観測時刻を、子要素 Initial に極性を記載する。</para>
		/// <para>（観測値）津波の最大波を観測したものの第１波を観測できなかった場合は、子要素 ArrivalTime 及び子要素 Initial に代わって子要素 Condition が出現し、ここに“第１波識別不能”と記載する。</para>
		/// <para>（予測値）当該潮位観測点への第１波の到達予想時刻を、子要素 ArrivalTime に記載する。</para>
		/// <para>（予測値）本情報の発表時点において、既に第１波が到達したと推測される場合や当該潮位観測点で第１波が観測された場合は、ArrivalTime に代わって子要素 Condition を追加し、それぞれ、“津波到達中と推測”、“第１波の到達を確認”を記載する。</para>
		/// <para>続報において、新たに本要素が出現する場合は子要素 Revise に“追加”を、既出であった本要素の内容が更新される場合は“更新”を記載する。</para>
		/// </remarks>
		public FirstHeight FirstHeight { get; set; }
		/// <summary>
		/// <para>（観測値）津波の最大波（観測値）</para>
		/// </summary>
		/// <remarks>
		/// <para>（観測値）観測したこれまでの最大波について 、子要素 DateTime に観測時刻を 、子要素 jmx_eb:TsunamiHeight に観測した津波の高さを記載する。</para>
		/// <para>（観測値）子要素jmx_eb:TsunamiHeightの @typeに“これまでの最大波の高さ”、@unitに津波の高さの単位である“m”、@description に文字列表現を記載する。また、これまでの最大波の高さが測定範囲を超え、「～以上」と表現する場合は、事例に示すとおり @descriptionに記載する。水位が上昇中の場合は、子要素 jmx_eb:TsunamiHeight に@condition が出現し、“上昇中”を記載する。</para>
		/// <para>（観測値）津波注意報の予報区（警報・注意報を解除した予報区も含む）において、観測されたこれまでの最大波が非常に小さい場合は、子要素 jmx_eb:TsunamiHeight に代わって子要素 Condition が出現し、ここに“微弱”と記載する。また、津波警報以上の津波予報区において、観測されたこれまでの最大波の高さが予想される高さに比べて十分小さい場合は、子要素 DateTime 及び子要素 jmx_eb:TsunamiHeight に代わって子要素 Condition が出現し、ここに“観測中”と記載する。</para>
		/// <para>（観測値）これまでの最大波の高さが大津波警報の基準を超え、追加あるいは更新された場合は、子要素 Condition を追加し、ここに“重要”と記載する。</para>
		/// <para>（観測値）続報において、新たに本要素が出現する場合は子要素 Revise に“追加”を、既出であった本要素の内容が更新される場合は“更新”を記載する。</para>
		/// <para>（観測値・沖合）ただし、Condition が“観測中”と記載されている場合で、且つ、前回が“観測中”であっても Revise に“更新”と記載している場合は、津波警報に相当する津波が観測されていることを示すので、注意する必要がある。具体的には、大津波警報が発表されている津波予報区に対応する沖合の潮位観測点において、観測値から推定される沿岸の津波の高さが大津波警報レベル（３ｍ超）に満たない場合、Conditionは“観測中”であるが Revise に“更新”を記載し、津波警報に相当する津波が観測されていることを示す。</para>
		/// </remarks>
		public MaxHeight MaxHeight { get; set; }
		public CurrentHeight CurrentHeight { get; set; }
	}

	public class FirstHeight {
		public XmlSerializedDateTimeOffset ArrivalTimeFrom { get; set; }
		public XmlSerializedDateTimeOffset ArrivalTimeTo { get; set; }
		public XmlSerializedDateTimeOffset ArrivalTime { get; set; }
		public string Condition { get; set; }
		public string Initial { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<float> TsunamiHeight { get; set; }
		public string Revise { get; set; }
		public float Period { get; set; }
	}

	public class MaxHeight {
		public XmlSerializedDateTimeOffset DateTime { get; set; }
		public string Condition { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<float> TsunamiHeightFrom { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<float> TsunamiHeightTo { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<float> TsunamiHeight { get; set; }
		public string Revise { get; set; }
		public float Period { get; set; }
	}

	public class CurrentHeight {
		public XmlSerializedDateTimeOffset StartTime { get; set; }
		public XmlSerializedDateTimeOffset EndTime { get; set; }
		public string Condition { get; set; }
		[XmlElement(Namespace = "http://xml.kishou.go.jp/jmaxml1/elementBasis1/")]
		public MeasuredValue<float> TsunamiHeight { get; set; }
	}
}
