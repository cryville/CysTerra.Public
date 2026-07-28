using Cryville.EEW.ComponentModel;
using System.ComponentModel;

namespace Cryville.EEW.FANStudio {
	public enum FANStudioSource {
		[EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] None,
		[LocalizableDisplayName(nameof(FSSNEarthquake), Path = ["SourceNameTypes"])] FSSNEarthquake = 0x100,
		[LocalizableDisplayName(nameof(FSSNCMT), Path = ["SourceNameTypes"])] FSSNCMT = 0x101,
	}
	public enum FANStudioAuthorizedSource {
		[EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] None,
		[LocalizableDisplayName(nameof(CMAWeatherAlarm), Path = ["SourceNameTypes"])] CMAWeatherAlarm = 0x10,
		[LocalizableDisplayName(nameof(NMEFCTsunamiWarning), Path = ["SourceNameTypes"])] NMEFCTsunamiWarning = 0x11,
		[LocalizableDisplayName(nameof(CENCEarthquake), Path = ["SourceNameTypes"])] CENCEarthquake = 0x20,
		[LocalizableDisplayName(nameof(CENCIntensityReport), Path = ["SourceNameTypes"])] CENCIntensityReport = 0x21,
		[LocalizableDisplayName(nameof(CEAEEW), Path = ["SourceNameTypes"])] CEAEEW = 0x28,
		[LocalizableDisplayName(nameof(CEAProvinceEEW), Path = ["SourceNameTypes"])] CEAProvinceEEW = 0x29,
		[LocalizableDisplayName(nameof(NingxiaEarthquake), Path = ["SourceNameTypes"])] NingxiaEarthquake = 0x30,
		[LocalizableDisplayName(nameof(GuangxiEarthquake), Path = ["SourceNameTypes"])] GuangxiEarthquake = 0x31,
		[LocalizableDisplayName(nameof(ShanxiEarthquake), Path = ["SourceNameTypes"])] ShanxiEarthquake = 0x32,
		[LocalizableDisplayName(nameof(BeijingEarthquake), Path = ["SourceNameTypes"])] BeijingEarthquake = 0x33,
		[LocalizableDisplayName(nameof(YunnanEarthquake), Path = ["SourceNameTypes"])] YunnanEarthquake = 0x34,
		[LocalizableDisplayName(nameof(CWAEarthquake), Path = ["SourceNameTypes"])] CWAEarthquake = 0x40,
		[LocalizableDisplayName(nameof(CWAEEW), Path = ["SourceNameTypes"])] CWAEEW = 0x48,
		[LocalizableDisplayName(nameof(JMAEEW), Path = ["SourceNameTypes"])] JMAEEW = 0x58,
		[LocalizableDisplayName(nameof(HKOEarthquake), Path = ["SourceNameTypes"])] HKOEarthquake = 0x60,
		[LocalizableDisplayName(nameof(USGSEarthquake), Path = ["SourceNameTypes"])] USGSEarthquake = 0x70,
		[LocalizableDisplayName(nameof(ShakeAlertEEW), Path = ["SourceNameTypes"])] ShakeAlertEEW = 0x78,
		[LocalizableDisplayName(nameof(EMSCEarthquake), Path = ["SourceNameTypes"])] EMSCEarthquake = 0x80,
		[LocalizableDisplayName(nameof(BCSFEarthquake), Path = ["SourceNameTypes"])] BCSFEarthquake = 0x81,
		[LocalizableDisplayName(nameof(GFZEarthquake), Path = ["SourceNameTypes"])] GFZEarthquake = 0x82,
		[LocalizableDisplayName(nameof(USPEarthquake), Path = ["SourceNameTypes"])] USPEarthquake = 0x83,
		[LocalizableDisplayName(nameof(KMAEarthquake), Path = ["SourceNameTypes"])] KMAEarthquake = 0x90,
		[LocalizableDisplayName(nameof(KMAEEW), Path = ["SourceNameTypes"])] KMAEEW = 0x98,
	}
}
