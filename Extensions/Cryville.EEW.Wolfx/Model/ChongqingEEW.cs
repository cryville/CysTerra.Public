using System;

namespace Cryville.EEW.Wolfx.Model {
	public record ChongqingEEW(
		string ID,
		string EventID,
		DateTime ReportTime,
		int ReportNum,
		DateTime OriginTime,
		string HypoCenter,
		float Latitude,
		float Longitude,
		float Magnitude,
		float? Depth,
		float MaxIntensity
	) : BaseModel;
}
