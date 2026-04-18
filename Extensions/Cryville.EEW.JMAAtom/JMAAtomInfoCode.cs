using Cryville.EEW.ComponentModel;
using System.ComponentModel;

namespace Cryville.EEW.JMAAtom {
	public enum JMAAtomInfoCode {
		[EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] None,
		[LocalizableDisplayName(nameof(VFVO50), Path = ["ControlTitle"])] VFVO50 = 0x0050,
		[LocalizableDisplayName(nameof(VFVO51), Path = ["ControlTitle"])] VFVO51 = 0x0051,
		[LocalizableDisplayName(nameof(VFVO52), Path = ["ControlTitle"])] VFVO52 = 0x0052,
		[LocalizableDisplayName(nameof(VFVO53), Path = ["ControlTitle"])] VFVO53 = 0x0053,
		[LocalizableDisplayName(nameof(VFVO54), Path = ["ControlTitle"])] VFVO54 = 0x0054,
		[LocalizableDisplayName(nameof(VFVO55), Path = ["ControlTitle"])] VFVO55 = 0x0055,
		[LocalizableDisplayName(nameof(VFVO56), Path = ["ControlTitle"])] VFVO56 = 0x0056,
		[LocalizableDisplayName(nameof(VFVO60), Path = ["ControlTitle"])] VFVO60 = 0x0060,
		[LocalizableDisplayName(nameof(VTSE41), Path = ["ControlTitle"])] VTSE41 = 0x0141,
		[LocalizableDisplayName(nameof(VTSE51), Path = ["ControlTitle"])] VTSE51 = 0x0151,
		[LocalizableDisplayName(nameof(VTSE52), Path = ["ControlTitle"])] VTSE52 = 0x0152,
		[LocalizableDisplayName(nameof(VXSE51), Path = ["ControlTitle"])] VXSE51 = 0x0251,
		[LocalizableDisplayName(nameof(VXSE52), Path = ["ControlTitle"])] VXSE52 = 0x0252,
		[LocalizableDisplayName(nameof(VXSE53), Path = ["ControlTitle"])] VXSE53 = 0x0253,
		[LocalizableDisplayName(nameof(VXSE56), Path = ["ControlTitle"])] VXSE56 = 0x0256,
		[LocalizableDisplayName(nameof(VXSE60), Path = ["ControlTitle"])] VXSE60 = 0x0260,
		[LocalizableDisplayName(nameof(VXSE61), Path = ["ControlTitle"])] VXSE61 = 0x0261,
		[LocalizableDisplayName(nameof(VXSE62), Path = ["ControlTitle"])] VXSE62 = 0x0262,
		[LocalizableDisplayName(nameof(VYSE50), Path = ["ControlTitle"])] VYSE50 = 0x0350,
		[LocalizableDisplayName(nameof(VYSE51VYSE52), Path = ["ControlTitle"])] VYSE51VYSE52 = 0x0351,
	}
}
