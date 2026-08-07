using Cryville.EEW.ComponentModel;

namespace Cryville.EEW.USGS {
	public enum USGSProductType {
		[LocalizableDisplayName("Origin", Path = ["ProductType"])] Origin = 0x0000,
		[LocalizableDisplayName("ShakeMapContMmi", Path = ["ProductType"])] ShakeMapContMmi = 0x0100,
		[LocalizableDisplayName("ShakeMapContPga", Path = ["ProductType"])] ShakeMapContPga = 0x0101,
		[LocalizableDisplayName("ShakeMapContPgv", Path = ["ProductType"])] ShakeMapContPgv = 0x0102,
	}
}
