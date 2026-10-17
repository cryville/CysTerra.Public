using Cryville.EEW.USGS.Model;
using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.MangxuProjectAPI.Model {
	public record EarthquakeEvent(
		string Id,
		string Agency,
		double Magnitude,
		string Location,
		double DepthKm,
		double Latitude,
		double Longitude,
		DateTime TimeUtc,
		long Timestamp
	) {
		public USGSEarthquakeSummary? SaRaw { get; set; }
		public CeaData? CeaData { get; set; }
		public EewRaw? EewRaw { get; set; }
		public CwaEewRaw? CwaEewRaw { get; set; }
		public CeicRaw? CeicRaw { get; set; }
		[JsonInclude]
		internal JMAQuake? P2pRaw { get; set; }
		public MmdRaw? MmdRaw { get; set; }
	}
}
