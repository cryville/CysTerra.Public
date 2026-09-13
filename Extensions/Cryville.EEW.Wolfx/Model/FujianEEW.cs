using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.Wolfx.Model {
	/// <summary>
	/// Represents an EEW from Fujian Earthquake Agency (福建地震局.)
	/// </summary>
	public record FujianEEW(
		string EventID,
		DateTime ReportTime,
		int ReportNum,
		DateTime OriginTime,
		string HypoCenter,
		float Latitude,
		float Longitude,
		float Magnitude,
		[property: JsonPropertyName("isFinal")] bool IsFinal
	) : BaseModel;
}
