using Cryville.EEW.Report;

namespace Cryville.EEW.USGS {
	sealed record USGSEventIDGroupKey(string EventID) : IReportGroupKey;
}
