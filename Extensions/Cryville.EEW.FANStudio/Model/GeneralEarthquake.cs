using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	public abstract record GeneralEarthquake(
		[property: JsonPropertyName("id")] string ID,
		[property: JsonPropertyName("shockTime")] DateTime ShockTime,
		[property: JsonPropertyName("latitude")] float Latitude,
		[property: JsonPropertyName("longitude")] float Longitude,
		[property: JsonPropertyName("depth")] float? Depth,
		[property: JsonPropertyName("magnitude")] float? Magnitude,
		[property: JsonPropertyName("placeName")] string PlaceName
	);
	public record EMSCEarthquake(string ID, DateTime ShockTime, float Latitude, float Longitude, float? Depth, float? Magnitude, string PlaceName) : GeneralEarthquake(ID, ShockTime, Latitude, Longitude, Depth, Magnitude, PlaceName);
	public record BCSFEarthquake(string ID, DateTime ShockTime, float Latitude, float Longitude, float? Depth, float? Magnitude, string PlaceName) : GeneralEarthquake(ID, ShockTime, Latitude, Longitude, Depth, Magnitude, PlaceName);
	public record GFZEarthquake(string ID, DateTime ShockTime, float Latitude, float Longitude, float? Depth, float? Magnitude, string PlaceName) : GeneralEarthquake(ID, ShockTime, Latitude, Longitude, Depth, Magnitude, PlaceName);
	public record USPEarthquake(string ID, DateTime ShockTime, float Latitude, float Longitude, float? Depth, float? Magnitude, string PlaceName) : GeneralEarthquake(ID, ShockTime, Latitude, Longitude, Depth, Magnitude, PlaceName);
}
