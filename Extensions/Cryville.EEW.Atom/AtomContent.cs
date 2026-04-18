using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.Atom {
	/// <summary>
	/// The content of an entry.
	/// </summary>
	public class AtomContent {
		/// <summary>
		/// One of "text", "html", or "xhtml", or a MIME media type (except a composite type).
		/// </summary>
		[XmlAttribute("type")] public string Type { get; set; }
		/// <summary>
		/// An IRI reference to the source.
		/// </summary>
		[XmlAttribute("src")] public string Source { get; set; }
		/// <summary>
		/// The content.
		/// </summary>
		[XmlText] public string Content { get; set; }

		/// <inheritdoc />
		public override string ToString() => Content;
	}
}
