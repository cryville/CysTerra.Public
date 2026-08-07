using Cryville.EEW.Report;

namespace Cryville.EEW.USGS {
	sealed record USGSProductUnitKey(string Source, string Type, string Code, string File) : IReportUnitKey;
}
