using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public record NMEFCTsunamiWarning(
		[property: JsonPropertyName("id")] string ID,
		[property: JsonPropertyName("code")] string Code,
		[property: JsonPropertyName("warningInfo")] NMEFCTsunamiWarningWarningInfo WarningInfo,
		[property: JsonPropertyName("timeInfo")] NMEFCTsunamiWarningTimeInfo TimeInfo,
		[property: JsonPropertyName("details")] NMEFCTsunamiWarningDetails Details,
		[property: JsonPropertyName("forecasts")] IReadOnlyCollection<NMEFCTsunamiWarningForecast> Forecasts,
		[property: JsonPropertyName("waterLevelMonitoring")] IReadOnlyCollection<NMEFCTsunamiWarningWaterLevelMonitoring> WaterLevelMonitoring
	);
	public record NMEFCTsunamiWarningWarningInfo(
		[property: JsonPropertyName("title")] string Title,
		[property: JsonPropertyName("level")] string Level,
		[property: JsonPropertyName("subtitle")] string Subtitle,
		[property: JsonPropertyName("orgUnit")] string OrgUnit
	);
	public record NMEFCTsunamiWarningTimeInfo(
		[property: JsonPropertyName("alarmDate")] string AlarmDate,
		[property: JsonPropertyName("updateDate")] string UpdateDate
	);
	public record NMEFCTsunamiWarningDetails(
		[property: JsonPropertyName("batch")] string Batch,
		[property: JsonPropertyName("logoUrl")] Uri LogoUrl,
		[property: JsonPropertyName("htmlUrl")] Uri HtmlUrl,
		[property: JsonPropertyName("maps")] NMEFCTsunamiWarningDetailsMaps Maps
	);
	public record NMEFCTsunamiWarningDetailsMaps(
		[property: JsonPropertyName("amplitudeMapUrl")] Uri AmplitudeMapUrl,
		[property: JsonPropertyName("coastalMapUrl")] Uri CoastalMapUrl
	);
	public record NMEFCTsunamiWarningForecast(
		[property: JsonPropertyName("province")] string Province,
		[property: JsonPropertyName("estimatedArrivalTime")] string EstimatedArrivalTime,
		[property: JsonPropertyName("maxWaveHeight")] string MaxWaveHeight,
		[property: JsonPropertyName("warningLevel")] string WarningLevel
	);
	public record NMEFCTsunamiWarningWaterLevelMonitoring(
		[property: JsonPropertyName("stationName")] string StationName,
		[property: JsonPropertyName("location")] string Location,
		[property: JsonPropertyName("coordinates")] NMEFCTsunamiWarningWaterLevelMonitoringCoordinates Coordinates,
		[property: JsonPropertyName("time")] string Time,
		[property: JsonPropertyName("maxWaveHeight")] string MaxWaveHeight
	);
	public record NMEFCTsunamiWarningWaterLevelMonitoringCoordinates(
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("longitude")] float Longitude
	);
}
