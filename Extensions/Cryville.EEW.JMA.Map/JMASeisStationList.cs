using Cryville.EEW.JMA.Map.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.JMA.Map {
	public class JMASeisStationList {
		static JMASeisStationList? s_instance;
		public static JMASeisStationList Instance => s_instance ??= new();

		public IReadOnlyCollection<JMASeisStation> Stations { get; private set; }
		public IReadOnlyDictionary<string, JMASeisArea> Areas { get; private set; }
		readonly Dictionary<string, JMASeisStation> _map;
		JMASeisStationList() {
			Stations = JsonSerializer.Deserialize(JMASeisStations.Data, SerializerContext.Default.IReadOnlyCollectionJMASeisStation)
				?? throw new FormatException("Invalid stations data.");
			Areas = JsonSerializer.Deserialize(JMASeisStations.Areas, SerializerContext.Default.IReadOnlyDictionaryStringJMASeisArea)
				?? throw new FormatException("Invalid areas data.");
			_map = Stations.ToDictionary(s => s.Code);
		}
		public JMASeisStation this[string code] => _map[code];
		public bool TryGet(string code, [NotNullWhen(true)] out JMASeisStation? station) => _map.TryGetValue(code, out station);
	}

	public record JMASeisCodedLocation<TCode>(
		[property: JsonPropertyName("code")] TCode Code,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("furigana")] string Furigana
	);
	public record JMASeisStation(
		[property: JsonPropertyName("pref")] JMASeisCodedLocation<int> Prefecture,
		[property: JsonPropertyName("area")] JMASeisCodedLocation<string> Area,
		[property: JsonPropertyName("city")] JMASeisCodedLocation<string> City,
		string Code,
		string Name,
		string Furigana,
		[property: JsonPropertyName("lat")] JsonElement RawLatitude,
		[property: JsonPropertyName("lon")] JsonElement RawLongitude,
		[property: JsonPropertyName("affi")] string Affiliation
	) : JMASeisCodedLocation<string>(Code, Name, Furigana) {
		public float Latitude => ToCoordinateValue(RawLatitude);
		public float Longitude => ToCoordinateValue(RawLongitude);
		static float ToCoordinateValue(JsonElement element) => element.ValueKind switch {
			JsonValueKind.Number => element.GetSingle(),
			JsonValueKind.String => float.Parse(element.GetString() ?? throw new InvalidOperationException("Empty coordinate value"), CultureInfo.InvariantCulture),
			_ => throw new InvalidOperationException("Unsupported coordinate value"),
		};
	}

	public record JMASeisArea(
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("furigana")] string Furigana,
		[property: JsonPropertyName("lat")] float Latitude,
		[property: JsonPropertyName("lon")] float Longitude,
		[property: JsonPropertyName("count")] int StationCount
	);
}
