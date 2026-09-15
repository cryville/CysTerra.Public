using Cryville.EEW.ComponentModel;

namespace Cryville.EEW.BMKGOpenData {
	public enum DataSubtype {
		[LocalizableDisplayName(nameof(Major), Path = ["DataSubtype"])] Major = 0,
		[LocalizableDisplayName(nameof(Felt), Path = ["DataSubtype"])] Felt = 1,
	}
}
