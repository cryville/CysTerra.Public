using Cryville.EEW.ComponentModel;
using System.ComponentModel;

namespace Cryville.EEW.MangxuProjectAPI {
	public enum MangxuAgency {
		[EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] None,
		[LocalizableDisplayName("sa_eew", Path = ["SourceNameTypes"])] SaEew = 0x10,
		[LocalizableDisplayName("cea", Path = ["SourceNameTypes"])] Cea = 0x11,
		[LocalizableDisplayName("cea_pr", Path = ["SourceNameTypes"])] CeaPr = 0x12,
		[LocalizableDisplayName("jma_eew", Path = ["SourceNameTypes"])] JmaEew = 0x13,
		[LocalizableDisplayName("cwa_eew", Path = ["SourceNameTypes"])] CwaEew = 0x14,
		[LocalizableDisplayName("ceic", Path = ["SourceNameTypes"])] Ceic = 0x20,
		[LocalizableDisplayName("usgs", Path = ["SourceNameTypes"])] Usgs = 0x21,
		[LocalizableDisplayName("emsc", Path = ["SourceNameTypes"])] Emsc = 0x22,
		[LocalizableDisplayName("jma", Path = ["SourceNameTypes"])] Jma = 0x23,
		[LocalizableDisplayName("bmkg", Path = ["SourceNameTypes"])] Bmkg = 0x24,
		[LocalizableDisplayName("geonet", Path = ["SourceNameTypes"])] Geonet = 0x25,
		[LocalizableDisplayName("ingv", Path = ["SourceNameTypes"])] Ingv = 0x26,
		[LocalizableDisplayName("gfz", Path = ["SourceNameTypes"])] Gfz = 0x27,
		[LocalizableDisplayName("cwa", Path = ["SourceNameTypes"])] Cwa = 0x28,
		[LocalizableDisplayName("hko", Path = ["SourceNameTypes"])] Hko = 0x29,
		[LocalizableDisplayName("kma", Path = ["SourceNameTypes"])] Kma = 0x2a,
		[LocalizableDisplayName("nrcan", Path = ["SourceNameTypes"])] Nrcan = 0x2b,
		[LocalizableDisplayName("bcsf", Path = ["SourceNameTypes"])] Bcsf = 0x2c,
		[LocalizableDisplayName("usp", Path = ["SourceNameTypes"])] Usp = 0x2d,
		[LocalizableDisplayName("mmd", Path = ["SourceNameTypes"])] Mmd = 0x2e,
	}
}
