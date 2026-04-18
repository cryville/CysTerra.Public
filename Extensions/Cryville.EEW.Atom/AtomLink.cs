using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.Atom {
	/// <summary>
	/// A reference from an entry or feed to a Web resource.
	/// </summary>
	public class AtomLink {
		/// <summary>
		/// The link's IRI.
		/// </summary>
		[XmlAttribute("href")] public string HRef { get; set; }
		/// <summary>
		/// The link relation type.
		/// </summary>
		[XmlAttribute("rel")] public string Rel { get; set; }
		/// <summary>
		/// An advisory media type: a hint about the type of the representation that is expected to be returned when the value of the <see cref="HRef" /> attribute is dereferenced.
		/// </summary>
		[XmlAttribute("type")] public string Type { get; set; }
		/// <summary>
		/// The language of the resource pointed to by the <see cref="HRef" /> attribute.
		/// </summary>
		[XmlAttribute("hreflang")] public string HRefLang { get; set; }
		/// <summary>
		/// Human-readable information about the link.
		/// </summary>
		[XmlAttribute("title")] public string Title { get; set; }
		/// <summary>
		/// An advisory length of the linked content in octets; a hint about the content length of the representation returned when the IRI in the <see cref="HRef" /> attribute is mapped to a URI and dereferenced.
		/// </summary>
		[XmlAttribute("length")] public long Length { get; set; } = -1;

		/// <inheritdoc />
		public override string ToString() => HRef;
	}
}
