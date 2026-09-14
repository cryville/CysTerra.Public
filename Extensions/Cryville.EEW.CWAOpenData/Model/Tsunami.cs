using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.CWAOpenData.Model {
	public record Tsunami(
		string ReportColor,
		string ReportContent,
		string ReportNo,
		string ReportType,
		DateTimeOffset IssueTime,
		ValidTime ValidTime,
		int TsunamiNo,
		string Web,
		EarthquakeInfo EarthquakeInfo,
		TsunamiWave TsunamiWave
	) : CWAReport(ReportType, ReportColor, ReportContent, IssueTime, ValidTime, Web);

	public record TsunamiWave(
		[property: JsonPropertyName("WarningArea")] WarningArea[] WarningAreas,
		[property: JsonPropertyName("TsuStation")] TsuStation[] TsuStations
	);

	public record WarningArea(
		string AreaColor,
		string AreaDesc,
		string AreaName,
		DateTimeOffset ArrivalTime,
		string InfoStatus,
		string WaveHeight
	);

	public record TsuStation(
		DateTimeOffset ArrivalTime,
		string InfoStatus,
		string StationID,
		string StationName,
		float StationLatitude,
		float StationLongitude,
		string WaveHeight
	);
}
