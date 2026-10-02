using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cryville.EEW.NowQuakeCENCInt.Model {
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
	[JsonDerivedType(typeof(WsHeartbeat), "heartbeat")]
	[JsonDerivedType(typeof(WsData), "data")]
	public record WsMessage;

	public record WsHeartbeat(
		[property: JsonPropertyName("time")] long Time,
		[property: JsonPropertyName("conn_id")] string ConnectionId
	) : WsMessage;

	public record WsData(
		[property: JsonPropertyName("eq_id")] string EarthquakeId,
		[property: JsonPropertyName("happen_time")] DateTime HappenTime,
		[property: JsonPropertyName("update_time")] DateTime UpdateTime,
		[property: JsonPropertyName("hypocenter")] string? Hypocenter,
		[property: JsonPropertyName("magnitude")] float Magnitude,
		[property: JsonPropertyName("depth")] float? Depth,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("maxintensity")] float? MaxIntensity,
		[property: JsonPropertyName("maxforecastintensity")] float? MaxForecastIntensity,
		[property: JsonPropertyName("info")] string? Info,
		[property: JsonPropertyName("stations")] IReadOnlyCollection<Station>? Stations,
		[property: JsonPropertyName("forecast")] string? Forecast
	) : WsMessage;

	public record Station(
		[property: JsonPropertyName("code")] string Code,
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("stationid")] string StationId,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("distance")] double Distance,
		[property: JsonPropertyName("int")] float Intensity,
		[property: JsonPropertyName("IA")] float? IntensityAcceleration,
		[property: JsonPropertyName("IV")] float? IntensityVelocity,
		[property: JsonPropertyName("forecast_int")] double? ForecastIntensity,
		[property: JsonPropertyName("int_diff")] double? IntensityDifference,
		[property: JsonPropertyName("pga")] float PGA,
		[property: JsonPropertyName("pgv")] float PGV,
		[property: JsonPropertyName("place")] string Place,
		[property: JsonPropertyName("vs30")] double Vs30,
		[property: JsonPropertyName("pgaEW")] double PGAEW,
		[property: JsonPropertyName("pgaNS")] double PGANS,
		[property: JsonPropertyName("pgaUD")] double PGAUD,
		[property: JsonPropertyName("pgvEW")] double PGVEW,
		[property: JsonPropertyName("pgvNS")] double PGVNS,
		[property: JsonPropertyName("pgvUD")] double PGVUD,
		[property: JsonPropertyName("location_name")] LocationName? LocationName
	);

	public record LocationName(
		[property: JsonPropertyName("province")] string? Province,
		[property: JsonPropertyName("city")] string? City,
		[property: JsonPropertyName("county")] string? County,
		[property: JsonPropertyName("town")] string? Town,
		[property: JsonPropertyName("placename")] string? PlaceName
	);
}
