using System;
using System.Globalization;

namespace Cryville.EEW.NowQuakeCENCInt {
	public static class Local {
		public static readonly CultureInfo Culture = SharedCultures.Get("zh-CN");
		public static readonly TimeSpan TimeZoneOffset = TimeSpan.FromHours(8);
		public static readonly TimeZoneInfo TimeZone = SharedTimeZones.GetTimeZone("China Standard Time", TimeZoneOffset);
	}
}