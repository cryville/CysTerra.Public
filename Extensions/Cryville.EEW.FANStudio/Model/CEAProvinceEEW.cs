using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record CEAProvinceEEW(
		string ID,
		string EventID,
		DateTime ShockTime,
		DateTime? UpdateTime,
		float Longitude,
		float Latitude,
		string PlaceName,
		float Magnitude,
		float? EpicenterIntensity,
		float? Depth,
		int Updates,
		[property: JsonPropertyName("province")] string Province
	) : CEAEEW(ID, EventID, ShockTime, UpdateTime, Longitude, Latitude, PlaceName, Magnitude, EpicenterIntensity, Depth, Updates) {
	}
}
