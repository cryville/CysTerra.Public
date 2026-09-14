using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Cryville.EEW.CWAOpenData {
	public static class SharedResources {
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string? SourceName(string subtype, [NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return string.Format(culture, res.GetStringRequired("SourceName"), res.GetStringSet("EventTypes")?.GetString(subtype));
		}
	}
}
