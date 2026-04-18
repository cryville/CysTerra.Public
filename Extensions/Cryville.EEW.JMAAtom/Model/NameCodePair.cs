#nullable disable

namespace Cryville.EEW.JMAAtom.Model {
	public class NameCodePair {
		public string Name { get; set; }
		public string Code { get; set; }

		public override string ToString() => Name;
	}
}
