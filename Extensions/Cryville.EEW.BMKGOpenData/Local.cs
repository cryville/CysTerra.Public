using System;
using System.Globalization;

namespace Cryville.EEW.BMKGOpenData {
	public static class Local {
		public static readonly CultureInfo Culture = SharedCultures.Get("id-ID");
		public static readonly TimeZoneInfo TimeZone = SharedTimeZones.GetTimeZone("SE Asia Standard Time", TimeSpan.FromHours(7));
	}
}
