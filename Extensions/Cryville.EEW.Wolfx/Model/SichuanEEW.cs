using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.Wolfx.Model {
	/// <summary>
	/// Represents an EEW from Sichuan Earthquake Administration (四川地震局.)
	/// </summary>
	public record SichuanEEW(
		int ID,
		string EventID,
		DateTime ReportTime,
		int ReportNum,
		DateTime OriginTime,
		string HypoCenter,
		float Latitude,
		float Longitude,
		[property: JsonPropertyName("Magunitude")] float Magnitude,
		float? Depth,
		float MaxIntensity
	) : BaseModel;
}
