using System;
using System.Globalization;

namespace Cryville.EEW.Wolfx {
	public static class Local {
		public static readonly CultureInfo Culture = SharedCultures.Get("zh-CN");
		public static readonly TimeSpan TimeZoneOffset = TimeSpan.FromHours(8);
		public static readonly TimeZoneInfo TimeZone = SharedTimeZones.GetTimeZone("China Standard Time", TimeZoneOffset);

		public static readonly CultureInfo TaiwanCulture = SharedCultures.Get("zh-TW");
		public static readonly TimeSpan TaiwanTimeZoneOffset = TimeSpan.FromHours(8);
		public static readonly TimeZoneInfo TaiwanTimeZone = SharedTimeZones.GetTimeZone("Taipei Standard Time", TaiwanTimeZoneOffset);

		public static readonly CultureInfo JapanCulture = SharedCultures.Get("ja-JP");
		public static readonly TimeSpan JapanTimeZoneOffset = TimeSpan.FromHours(9);
		public static readonly TimeZoneInfo JapanTimeZone = SharedTimeZones.GetTimeZone("Tokyo Standard Time", JapanTimeZoneOffset);
	}
}
