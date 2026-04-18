using System.Xml.Serialization;

#nullable disable

namespace Cryville.EEW.Atom {
	/// <summary>
	/// A person, corporation, or similar entity.
	/// </summary>
	public class AtomPerson {
		/// <summary>
		/// A human-readable name for the person.
		/// </summary>
		[XmlElement("name")] public string Name { get; set; }

		/// <inheritdoc />
		public override string ToString() => Name;
	}
}
