using System;
using System.Globalization;

namespace Cryville.EEW.GeoNet {
	public static class Local {
		public static readonly CultureInfo Culture = SharedCultures.Get("en-NZ");
		public static readonly TimeZoneInfo TimeZone = SharedTimeZones.GetTimeZone("New Zealand Standard Time", TimeSpan.FromHours(12));
	}
}
