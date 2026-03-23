using Cryville.EEW.GeoJSON;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.EMSC {
	public sealed class EMSCRealTimeAction {
		[JsonPropertyName("action")]
		public string Action { get; init; }

		[JsonPropertyName("data")]
		public JsonElement Data { get; init; }

		[JsonIgnore]
		public Feature<EMSCRealTimeEvent>? Feature { get; }

		[JsonIgnore]
		public EMSCRealTimeEvent? Event => Feature?.Properties;

		[JsonIgnore]
		public string? EventID { get; }

		public EMSCRealTimeAction(string Action, JsonElement Data) {
			this.Action = Action;
			this.Data = Data;
			if (Data.ValueKind == JsonValueKind.Object) {
				Feature = JsonSerializer.Deserialize(Data, SerializerContext.Default.FeatureEMSCRealTimeEvent) ?? throw new InvalidOperationException("Invalid event data.");
			}
			else if (Data.ValueKind == JsonValueKind.String) {
				EventID = Data.GetString();
			}
		}
	}

	public record EMSCRealTimeEvent(
		[property: JsonPropertyName("source_id")] string SourceID,
		[property: JsonPropertyName("source_catalog")] string SourceCatalog,
		[property: JsonPropertyName("lastupdate")] DateTimeOffset LastUpdate,
		[property: JsonPropertyName("time")] DateTimeOffset Time,
		[property: JsonPropertyName("flynn_region")] string FlynnRegion,
		[property: JsonPropertyName("lat")] float Latitude,
		[property: JsonPropertyName("lon")] float Longitude,
		[property: JsonPropertyName("depth")] float Depth,
		[property: JsonPropertyName("evtype")] string EventType,
		[property: JsonPropertyName("auth")] string Authority,
		[property: JsonPropertyName("mag")] float Magnitude,
		[property: JsonPropertyName("magtype")] string MagnitudeType,
		[property: JsonPropertyName("unid")] string UniqueID
	);
}
