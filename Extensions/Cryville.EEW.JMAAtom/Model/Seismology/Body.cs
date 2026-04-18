using System.Collections.ObjectModel;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model.Seismology {
	/// <summary>
	/// 内容部
	/// </summary>
	/// <remarks>
	/// <para>本情報の量的な詳細内容を記載する。</para>
	/// </remarks>
	public class Body {
		public Naming Naming { get; set; }
		/// <summary>
		/// 津波
		/// </summary>
		/// <remarks>
		/// <para>津波に関連する情報を記載する。</para>
		/// <para>ヘッダ部の「情報形態」（Head/InfoType）が“取消”の場合、本要素は出現しない。</para>
		/// </remarks>
		public Tsunami Tsunami { get; set; }
		[XmlElement("Earthquake")]
		public Collection<Earthquake> Earthquakes { get; set; }
		/// <summary>
		/// 震度
		/// </summary>
		/// <remarks>
		/// <para>震度に関する情報を記載する。</para>
		/// <para>ヘッダ部の「情報形態」（Head/InfoType）が“取消”の場合、本要素は出現しない。</para>
		/// </remarks>
		public Intensity Intensity { get; set; }
		/// <summary>
		/// 地震関連情報
		/// </summary>
		/// <remarks>
		/// <para>情報に関する諸要素を記載する。</para>
		/// <para>情報形態（Head/InfoType）が“取消”の場合、本要素は出現しない。</para>
		/// </remarks>
		public EarthquakeInfo EarthquakeInfo { get; set; }
		/// <summary>
		/// 地震回数
		/// </summary>
		/// <remarks>
		/// <para>具体的な地震回数を発表する場合に本要素が出現し、ここに具体的回数を記載する。</para>
		/// <para>ヘッダ部の「情報形態」（Head/InfoType）が“取消”の場合、本要素は出現しない。</para>
		/// </remarks>
		public EarthquakeCount EarthquakeCount { get; set; }
		//public Aftershocks Aftershock { get; set; }
		/// <summary>
		/// テキスト要素
		/// </summary>
		/// <remarks>
		/// <para>自由文形式で追加的に情報を記載する必要がある場合等に、本要素を用いて記載する。例えば、ヘッダ部の「情報形態」（Head/InfoType）が“取消”の場合に、取消しの概要等を本要素に記載する。</para>
		/// </remarks>
		public string Text { get; set; }
		/// <summary>
		/// 次回発表予定
		/// </summary>
		/// <remarks>
		/// <para>次回の情報発表予定時刻等に関する情報（情報発表の終了を含む）を記載する。</para>
		/// <para>情報形態（Head/InfoType）が“取消”の場合、本要素は出現しない。</para>
		/// </remarks>
		public string NextAdvisory { get; set; }
		/// <summary>
		/// 付加文
		/// </summary>
		/// <remarks>
		/// <para>情報の本文に加えて付加的な情報を記載する必要がある場合は、本要素以下に情報を記載する。</para>
		/// <para>ヘッダ部の「情報形態」（Head/InfoType）が“取消”の場合、本要素は出現しない。</para>
		/// </remarks>
		public Comment Comments { get; set; }
	}

	public class Naming {
		[XmlText]
		public string Native { get; set; }
		[XmlAttribute("english")]
		public string English { get; set; }
	}

	/// <summary>
	/// コード体系の定義
	/// </summary>
	public class CodeDefine {
		[XmlElement("Type")]
		public Collection<CodeDefineType> Types { get; set; }
	}

	public class CodeDefineType {
		/// <summary>
		/// コードの種類に応じる子要素
		/// </summary>
		[XmlText]
		public string Value { get; set; }
		/// <summary>
		/// 定義したコードを使用する要素の相対的な出現位置
		/// </summary>
		[XmlAttribute("xpath")]
		public string XPath { get; set; }
	}

	public class Category {
		public NameCodePair Kind { get; set; }
		public NameCodePair LastKind { get; set; }
	}
}
