using Cryville.EEW.Report;

namespace Cryville.EEW.GeoNet {
	sealed record GeoNetPublicIDGroupKey(string PublicID) : IReportGroupKey;
}
