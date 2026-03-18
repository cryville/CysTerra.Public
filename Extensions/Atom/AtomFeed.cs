using Cryville.EEW;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

#nullable disable

namespace Atom {
	/// <summary>
	/// An Atom Feed Document, acting as a container for metadata and data associated with the feed.
	/// </summary>
	[XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
	public class AtomFeed {
		/// <summary>
		/// A human-readable title for the feed.
		/// </summary>
		[XmlElement("title")] public string Title { get; set; }
		/// <summary>
		/// A human-readable description or subtitle for the feed.
		/// </summary>
		[XmlElement("subtitle")] public string Subtitle { get; set; }
		/// <summary>
		/// A permanent, universally unique identifier for the feed.
		/// </summary>
		[XmlElement("id")] public string Id { get; set; }
		/// <summary>
		/// The most recent instant in time when the feed was modified in a way the publisher considers significant.
		/// </summary>
		[XmlElement("updated")] public XmlSerializedDateTimeOffset Updated { get; set; }
		/// <summary>
		/// The authors of the feed.
		/// </summary>
		[XmlElement("author")] public Collection<AtomPerson> Authors { get; set; }
		/// <summary>
		/// References from the feed to Web resources.
		/// </summary>
		[XmlElement("link")] public Collection<AtomLink> Links { get; set; }
		/// <summary>
		/// Individual entries of the feed.
		/// </summary>
		[XmlElement("entry")] public Collection<AtomEntry> Entries { get; set; }
	}
}
