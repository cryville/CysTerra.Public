using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Cryville.EEW.CWAOpenData.Model {
	[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
	[SuppressMessage("Design", "CA1054", Justification = "Discard")]
	[SuppressMessage("Design", "CA1056", Justification = "Discard")]
	public record Earthquake(
		int EarthquakeNo,
		string ReportType,
		string ReportColor,
		string ReportContent,
		string ReportImageURI,
		string ReportRemark,
		DateTimeOffset IssueTime,
		ValidTime ValidTime,
		string Web,
		string? ShakemapImageURI,
		EarthquakeInfo EarthquakeInfo,
		Intensity Intensity
	) : CWAReport(ReportType, ReportColor, ReportContent, IssueTime, ValidTime, Web);

	public record EarthquakeInfo(
		DateTimeOffset OriginTime,
		string Source,
		float FocalDepth,
		Epicenter Epicenter,
		EarthquakeMagnitude EarthquakeMagnitude
	);

	public record Epicenter(
		string Location,
		float EpicenterLatitude,
		float EpicenterLongitude
	);

	public record EarthquakeMagnitude(
		string? MagnitudeType,
		float MagnitudeValue
	);

	public record Intensity(
		[property: JsonPropertyName("ShakingArea")] ShakingArea[] ShakingAreas
	);

	public record ShakingArea(
		string AreaDesc,
		string CountyName,
		string? InfoStatus,
		string AreaIntensity,
		[property: JsonPropertyName("EqStation")] EqStation[] EqStations
	);

	[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
	[SuppressMessage("Design", "CA1054", Justification = "Discard")]
	[SuppressMessage("Design", "CA1056", Justification = "Discard")]
	public record EqStation(
		[property: JsonPropertyName("pga")] GroundMotionVector? PGA,
		[property: JsonPropertyName("pgv")] GroundMotionVector? PGV,
		string StationName,
		string StationID,
		string? InfoStatus,
		float BackAzimuth,
		float EpicenterDistance,
		string SeismicIntensity,
		float StationLatitude,
		float StationLongitude,
		string? WaveImageURI
	);

	public record GroundMotionVector(
		[property: JsonPropertyName("unit")] string Unit,
		float EWComponent,
		float NSComponent,
		float VComponent,
		float IntScaleValue
	);
}
