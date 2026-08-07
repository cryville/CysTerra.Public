using Cryville.EEW.ComponentModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Cryville.EEW.USGS {
	[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
	[SuppressMessage("Design", "CA1027", Justification = "Not flags")]
	public enum USGSFeedType {
		[LocalizableDisplayName("significant_hour", Path = ["FeedType"])] SignificantHour = 0x00,
		[LocalizableDisplayName("4.5_hour", Path = ["FeedType"])] M45Hour = 0x01,
		[LocalizableDisplayName("2.5_hour", Path = ["FeedType"])] M25Hour = 0x02,
		[LocalizableDisplayName("1.0_hour", Path = ["FeedType"])] M10Hour = 0x03,
		[LocalizableDisplayName("all_hour", Path = ["FeedType"])] AllHour = 0x04,

		[LocalizableDisplayName("significant_day", Path = ["FeedType"])] SignificantDay = 0x10,
		[LocalizableDisplayName("4.5_day", Path = ["FeedType"])] M45Day = 0x11,
		[LocalizableDisplayName("2.5_day", Path = ["FeedType"])] M25Day = 0x12,
		[LocalizableDisplayName("1.0_day", Path = ["FeedType"])][EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] M10Day = 0x13,
		[LocalizableDisplayName("all_day", Path = ["FeedType"])][EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] AllDay = 0x14,

		[LocalizableDisplayName("significant_week", Path = ["FeedType"])] SignificantWeek = 0x20,
		[LocalizableDisplayName("4.5_week", Path = ["FeedType"])] M45Week = 0x21,
		[LocalizableDisplayName("2.5_week", Path = ["FeedType"])][EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] M25Week = 0x22,
		[LocalizableDisplayName("1.0_week", Path = ["FeedType"])][EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] M10Week = 0x23,
		[LocalizableDisplayName("all_week", Path = ["FeedType"])][EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] AllWeek = 0x24,

		[LocalizableDisplayName("significant_month", Path = ["FeedType"])] SignificantMonth = 0x30,
		[LocalizableDisplayName("4.5_month", Path = ["FeedType"])][EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] M45Month = 0x31,
		[LocalizableDisplayName("2.5_month", Path = ["FeedType"])][EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] M25Month = 0x32,
		[LocalizableDisplayName("1.0_month", Path = ["FeedType"])][EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] M10Month = 0x33,
		[LocalizableDisplayName("all_month", Path = ["FeedType"])][EditorBrowsable(EditorBrowsableState.Never)][Browsable(false)] AllMonth = 0x34,
	}
}
