using System;
using System.Globalization;

namespace Cryville.EEW.CWAOpenData {
	public static class Local {
		public static readonly CultureInfo Culture = SharedCultures.Get("zh-TW");
		public static readonly TimeSpan TimeZoneOffset = TimeSpan.FromHours(8);
		public static readonly TimeZoneInfo TimeZone = SharedTimeZones.GetTimeZone("Taipei Standard Time", TimeZoneOffset);
	}
}
