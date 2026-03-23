using System;
using System.Collections.Generic;

namespace Cryville.EEW.EMSC {
	sealed class SeismicParameterEqualityComparer : IEqualityComparer<EMSCRealTimeEvent> {
		static SeismicParameterEqualityComparer? s_instance;
		public static SeismicParameterEqualityComparer Instance => s_instance ??= new();

		public bool Equals(EMSCRealTimeEvent? x, EMSCRealTimeEvent? y) => x is null ? (y is null) : (y is not null && (
			x.SourceID.Equals(y.SourceID, StringComparison.Ordinal) &&
			x.SourceCatalog.Equals(y.SourceCatalog, StringComparison.Ordinal) &&
			x.Time.Equals(y.Time) &&
			x.Latitude.Equals(y.Latitude) &&
			x.Longitude.Equals(y.Longitude) &&
			x.Depth.Equals(y.Depth) &&
			x.EventType.Equals(y.EventType, StringComparison.Ordinal) &&
			x.Authority.Equals(y.Authority, StringComparison.Ordinal) &&
			x.Magnitude.Equals(y.Magnitude) &&
			x.MagnitudeType.Equals(y.MagnitudeType, StringComparison.Ordinal)
		));
		public int GetHashCode(EMSCRealTimeEvent obj) => HashCode.Combine(
			HashCode.Combine(obj.SourceID, obj.SourceCatalog, obj.EventType, obj.Authority),
			HashCode.Combine(obj.Time, obj.Latitude, obj.Longitude, obj.Depth, obj.Magnitude, obj.MagnitudeType)
		);
	}
}
