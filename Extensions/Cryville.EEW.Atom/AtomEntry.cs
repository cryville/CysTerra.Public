using System.Collections.ObjectModel;
using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.Atom {
	/// <summary>
	/// An individual entry, acting as a container for metadata and data associated with the entry.
	/// </summary>
	public class AtomEntry {
		/// <summary>
		/// A human-readable title for the entry.
		/// </summary>
		[XmlElement("title")] public string Title { get; set; }
		/// <summary>
		/// A permanent, universally unique identifier for the entry.
		/// </summary>
		[XmlElement("id")] public string Id { get; set; }
		/// <summary>
		/// The most recent instant in time when the entry was modified in a way the publisher considers significant.
		/// </summary>
		[XmlElement("updated")] public XmlSerializedDateTimeOffset Updated { get; set; }
		/// <summary>
		/// The authors of the entry.
		/// </summary>
		[XmlElement("author")] public Collection<AtomPerson> Authors { get; set; }
		/// <summary>
		/// References from the entry to Web resources.
		/// </summary>
		[XmlElement("link")] public Collection<AtomLink> Links { get; set; }
		/// <summary>
		/// The content of the entry.
		/// </summary>
		[XmlElement("content")] public AtomContent Content { get; set; }

		/// <inheritdoc />
		public override string ToString() => $"{Title}: {Content}";
	}
}
