using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Cryville.EEW.MangxuProjectAPI.Model {
	public record EewRaw(
		[property: JsonPropertyName("EventID")] string EventID,
		[property: JsonPropertyName("Flag")] EewRawFlag Flag,
		[property: JsonPropertyName("Forecast")] IList<EewRawForecast> Forecast,
		[property: JsonPropertyName("Hypocenter")] EewRawHypocenter Hypocenter,
		[property: JsonPropertyName("Intensity")] string Intensity,
		[property: JsonPropertyName("Magnitude")] string Magnitude,
		[property: JsonPropertyName("OriginDateTime")] DateTimeOffset OriginDateTime,
		[property: JsonPropertyName("ReportDateTime")] DateTimeOffset ReportDateTime,
		[property: JsonPropertyName("Serial")] int Serial,
		[property: JsonPropertyName("Text")] string Text,
		[property: JsonPropertyName("Title")] string Title
	);
	public record EewRawFlag(
		[property: JsonPropertyName("is_cancel")] bool IsCancel,
		[property: JsonPropertyName("is_final")] bool IsFinal,
		[property: JsonPropertyName("is_training")] bool IsTraining
	);
	public record EewRawForecast(
		[property: JsonPropertyName("Code")] int Code,
		[property: JsonPropertyName("Intensity")] EewRawForecastIntensity Intensity,
		[property: JsonPropertyName("Name")] string Name
	);
	public record EewRawForecastIntensity(
		[property: JsonPropertyName("Description")] string Description,
		[property: JsonPropertyName("From")] string From,
		[property: JsonPropertyName("To")] string To
	);
	[SuppressMessage("Performance", "CA1819", Justification = "DTO type")]
	public record EewRawHypocenter(
		[property: JsonPropertyName("Code")] int Code,
		[property: JsonPropertyName("Coordinate")] float[] Coordinate,
		[property: JsonPropertyName("Depth")] string Depth,
		[property: JsonPropertyName("Description")] string Description,
		[property: JsonPropertyName("Name")] string Name
	);
}
