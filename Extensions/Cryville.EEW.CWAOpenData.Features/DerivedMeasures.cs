using Cryville.Measure;

namespace Cryville.EEW.CWAOpenData.Features {
	static class DerivedMeasures {
		public static readonly Unit Kilometre = Units.Metre.WithPrefix(MetricPrefixes.Kilo);
		public static readonly Unit Centimetre = Units.Metre.WithPrefix(MetricPrefixes.Centi);
		public static readonly Unit Gal = Centimetre / Units.Second / Units.Second;
		public static readonly Unit Kine = Centimetre / Units.Second;
	}
}
