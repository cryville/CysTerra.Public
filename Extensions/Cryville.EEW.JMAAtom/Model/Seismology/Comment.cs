using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.JMAAtom.Model.Seismology {
	public class Comment {
		public CommentForm WarningComment { get; set; }
		/// <summary>
		/// 固定付加文
		/// </summary>
		/// <remarks>
		/// <para>津波や緊急地震速報に関する付加的な情報を、固定付加文の形式で子要素 Text に、また、対応するコードを子要素 Code に記載する。具体的なコードの値については、別途提供するコード表を参照。@codeType には“固定付加文”を記載する。</para>
		/// </remarks>
		public CommentForm ForecastComment { get; set; }
		public CommentForm ObservationComment { get; set; }
		public CommentForm VarComment { get; set; }
		/// <summary>
		/// 自由付加文
		/// </summary>
		/// <remarks>
		/// <para>その他の付加的な情報を、自由付加文の形式で記載する。</para>
		/// </remarks>
		public string FreeFormComment { get; set; }
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		[SuppressMessage("Design", "CA1056", Justification = "Unsupported XML serialization")]
		public string URI { get; set; }
	}

	public class CommentForm {
		public string Text { get; set; }
		public string Code { get; set; }
		[XmlAttribute("codeType")]
		public string CodeType { get; set; }

		public override string ToString() => Text;
	}
}