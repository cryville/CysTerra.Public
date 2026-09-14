using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.CWAOpenData.Model {
	public record CWAResult(
		[property: JsonPropertyName("success")] string Success,
		[property: JsonPropertyName("records")] CWARecords Records
	);

	public record CWARecords(
		[property: JsonPropertyName("Earthquake")] Earthquake[] Earthquakes,
		[property: JsonPropertyName("Tsunami")] Tsunami[] Tsunami
	);

	public record CWAReport(
		string ReportType,
		string ReportColor,
		string ReportContent,
		DateTimeOffset IssueTime,
		ValidTime ValidTime,
		string Web
	);
	public record ValidTime(
		DateTimeOffset EndTime
	);
}
