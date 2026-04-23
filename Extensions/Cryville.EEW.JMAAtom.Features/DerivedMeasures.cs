using Cryville.Measure;

namespace Cryville.EEW.JMAAtom.Features {
	static class DerivedMeasures {
		public static Unit CentimetrePreSecond = Units.Metre.WithPrefix(MetricPrefixes.Centi) / Units.Second;
	}
}
