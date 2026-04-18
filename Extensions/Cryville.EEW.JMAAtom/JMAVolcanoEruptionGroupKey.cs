using Cryville.EEW.Report;
using System;

namespace Cryville.EEW.JMAAtom {
	sealed record JMAVolcanoEruptionGroupKey(string Code, DateTime Time) : IReportGroupKey { }
}
