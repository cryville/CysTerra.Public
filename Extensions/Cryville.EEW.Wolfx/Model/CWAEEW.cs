using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.Wolfx.Model {
	public record CWAEEW(
		int ID,
		DateTime ReportTime,
		int ReportNum,
		DateTime OriginTime,
		string HypoCenter,
		float Latitude,
		float Longitude,
		float Magnitude,
		float Depth,
		string MaxIntensity,
		[property: JsonPropertyName("isCancel")] bool IsCancellation
	) : BaseModel;
}
