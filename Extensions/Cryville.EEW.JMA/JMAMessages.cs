using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Cryville.EEW.JMA {
	public static class JMAMessages {
		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizedResource AreaEpicenter([NotNull] ref CultureInfo? culture) => new(nameof(AreaEpicenter), ref culture);
		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizedResource AreaForecastEEW([NotNull] ref CultureInfo? culture) => new(nameof(AreaForecastEEW), ref culture);
		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizedResource AreaTsunami([NotNull] ref CultureInfo? culture) => new(nameof(AreaTsunami), ref culture);
		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizedResource PointTsunami([NotNull] ref CultureInfo? culture) => new(nameof(PointTsunami), ref culture);
		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizedResource PointVolcano([NotNull] ref CultureInfo? culture) => new(nameof(PointVolcano), ref culture);

		public static LocalizedResource AreaEpicenter(CultureInfo? culture) => AreaEpicenter(ref culture);
		public static LocalizedResource AreaForecastEEW(CultureInfo? culture) => AreaForecastEEW(ref culture);
		public static LocalizedResource AreaTsunami(CultureInfo? culture) => AreaTsunami(ref culture);
		public static LocalizedResource PointTsunami(CultureInfo? culture) => PointTsunami(ref culture);
		public static LocalizedResource PointVolcano(CultureInfo? culture) => PointVolcano(ref culture);

		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizableResource AreaEpicenter() => new(nameof(AreaEpicenter));
		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizableResource AreaForecastEEW() => new(nameof(AreaForecastEEW));
		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizableResource AreaTsunami() => new(nameof(AreaTsunami));
		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizableResource PointTsunami() => new(nameof(PointTsunami));
		[MethodImpl(MethodImplOptions.NoInlining)] public static LocalizableResource PointVolcano() => new(nameof(PointVolcano));
	}
}
