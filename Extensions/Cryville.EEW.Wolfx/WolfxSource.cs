using Cryville.EEW.ComponentModel;
using System.ComponentModel;

namespace Cryville.EEW.Wolfx {
	public enum WolfxSource {
		[EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] None,
		[LocalizableDisplayName(nameof(SichuanEEW), Path = ["SourceNameTypes"])] SichuanEEW = 0x01,
		[LocalizableDisplayName(nameof(JMAEEW), Path = ["SourceNameTypes"])] JMAEEW = 0x02,
		[LocalizableDisplayName(nameof(FujianEEW), Path = ["SourceNameTypes"])] FujianEEW = 0x03,
		[LocalizableDisplayName(nameof(CWAEEW), Path = ["SourceNameTypes"])] CWAEEW = 0x04,
		[LocalizableDisplayName(nameof(ChongqingEEW), Path = ["SourceNameTypes"])] ChongqingEEW = 0x05,
		[LocalizableDisplayName(nameof(CENCEEW), Path = ["SourceNameTypes"])] CENCEEW = 0x06,
		[LocalizableDisplayName(nameof(CENCEarthquake), Path = ["SourceNameTypes"])] CENCEarthquake = 0x11,
		[LocalizableDisplayName(nameof(JMAEarthquake), Path = ["SourceNameTypes"])] JMAEarthquake = 0x12,
	}
}
