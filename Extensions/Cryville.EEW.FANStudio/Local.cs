using System;
using System.Globalization;

namespace Cryville.EEW.FANStudio {
	public static class Local {
		public static readonly CultureInfo Culture = SharedCultures.Get("zh-CN");
		public static readonly TimeSpan TimeZoneOffset = TimeSpan.FromHours(8);
		public static readonly TimeZoneInfo TimeZone = SharedTimeZones.GetTimeZone("China Standard Time", TimeZoneOffset);

		public static readonly CultureInfo HongKongCulture = SharedCultures.Get("zh-HK");

		public static readonly CultureInfo TaiwanCulture = SharedCultures.Get("zh-TW");
		public static readonly TimeZoneInfo TaiwanTimeZone = SharedTimeZones.GetTimeZone("Taipei Standard Time", TimeSpan.FromHours(8));

		public static readonly CultureInfo SouthKoreaCulture = SharedCultures.Get("ko-KR");
		public static readonly TimeSpan SouthKoreaTimeZoneOffset = TimeSpan.FromHours(9);
		public static readonly TimeZoneInfo SouthKoreaTimeZone = SharedTimeZones.GetTimeZone("Korea Standard Time", SouthKoreaTimeZoneOffset);

		public static readonly CultureInfo UnitedStatesCulture = SharedCultures.Get("en-US");

		public static readonly CultureInfo GreatBritainCulture = SharedCultures.Get("en-GB");
	}
}
