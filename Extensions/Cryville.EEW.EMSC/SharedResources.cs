using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Cryville.EEW.EMSC {
	public static class SharedResources {
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string? SourceName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("SourceName");
		}
	}
}
