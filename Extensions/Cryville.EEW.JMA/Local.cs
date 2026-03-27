using System;
using System.Globalization;

namespace Cryville.EEW.JMA {
	public static class Local {
		public static readonly CultureInfo Culture = SharedCultures.Get("ja-JP");
		public static readonly CultureInfo CultureHrkt = SharedCultures.Get("ja-Hrkt-JP");
		public static readonly TimeZoneInfo TimeZone = SharedTimeZones.GetTimeZone("Tokyo Standard Time", TimeSpan.FromHours(9));
	}
}
