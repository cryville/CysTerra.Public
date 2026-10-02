using Cryville.Measure;

namespace Cryville.EEW.NowQuakeCENCInt.Features {
	static class DerivedMeasures {
		public static Unit Kilometre = Units.Metre.WithPrefix(MetricPrefixes.Kilo);
		public static readonly Unit Centimetre = Units.Metre.WithPrefix(MetricPrefixes.Centi);
		public static readonly Unit CentimetrePerSecond = Centimetre / Units.Second;
		public static readonly Unit CentimetrePerSecondSquared = CentimetrePerSecond / Units.Second;
	}
}
