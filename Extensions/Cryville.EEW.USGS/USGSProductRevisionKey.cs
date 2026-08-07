using Cryville.EEW.Report;
using System;

namespace Cryville.EEW.USGS {
	sealed record USGSProductRevisionKey(long LastModified) : IReportRevisionKey {
		public int CompareTo(IReportRevisionKey? obj) {
			if (obj is not USGSProductRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
			return LastModified.CompareTo(other.LastModified);
		}
	}
}
