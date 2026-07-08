using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Cryville.EEW.Wolfx {
	public static class SharedResources {
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string? SourceName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("SourceName");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string? SourceName(string type, [NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return string.Format(culture, res.GetStringRequired("SourceNameTyped"), res.GetStringSetRequired("SourceNameTypes").GetString(type));
		}
	}
}
