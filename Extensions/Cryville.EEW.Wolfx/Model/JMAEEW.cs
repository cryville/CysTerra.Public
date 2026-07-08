using Cryville.Common.Compat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Cryville.EEW.Wolfx.Model {
	/// <summary>
	/// Represents an EEW (緊急地震速報) from Japan Meteorological Agency (国土交通省気象庁.)
	/// </summary>
	public record JMAEEW : BaseModel {
		public string OriginalText { get; private init; }
		public JMAEEWType Type { get; private set; }
		public JMAEEWPublishingOffice PublishingOffice { get; private set; }
		public JMAEEWStatus Status { get; private set; }
		public DateTime DateTime { get; private set; }
		public DateTime OriginTime { get; private set; }
		public string? EventId { get; private set; }
		public JMAEEWSerialType? SerialType { get; private set; }
		public int? Serial { get; private set; }
		public string? HypocenterCode { get; private set; }
		public float? Latitude { get; private set; }
		public float? Longitude { get; private set; }
		public int? Depth { get; private set; }
		public float? Magnitude { get; private set; }
		public string? MaxIntensity { get; private set; }
		public JMAEEWHypocenterAccuracy? HypocenterAccuracy { get; private set; }
		public JMAEEWHypocenterAccuracy? DepthAccuracy { get; private set; }
		public JMAEEWMagnitudeAccuracy? MagnitudeAccuracy { get; private set; }
		public JMAEEWLandOrSea? LandOrSea { get; private set; }
		public JMAEEWForecastType? ForecastType { get; private set; }
		public JMAEEWForecastMethod? ForecastMethod { get; private set; }
		public JMAEEWMaxIntensityChange? MaxIntensityChange { get; private set; }
		public JMAEEWMaxIntensityChangeReason? MaxIntensityChangeReason { get; private set; }
		readonly Collection<JMAEEWForecastArea> m_forecastAreas = [];
		public IReadOnlyCollection<JMAEEWForecastArea> ForecastAreas => m_forecastAreas;

		static readonly char[] _seps = [' ', '\n'];
		public JMAEEW(string OriginalText) {
			ThrowHelper.ThrowIfNullOrWhiteSpace(OriginalText);
			this.OriginalText = OriginalText;
			var data = OriginalText.Split(_seps, StringSplitOptions.RemoveEmptyEntries);
			Type = (JMAEEWType)int.Parse(data[0], CultureInfo.InvariantCulture);
			PublishingOffice = (JMAEEWPublishingOffice)int.Parse(data[1], CultureInfo.InvariantCulture);
			Status = (JMAEEWStatus)int.Parse(data[2], CultureInfo.InvariantCulture);
			DateTime = DateTime.ParseExact(data[3], "yyMMddHHmmss", CultureInfo.InvariantCulture);
			OriginTime = DateTime.ParseExact(data[5], "yyMMddHHmmss", CultureInfo.InvariantCulture);
			int i = 6;
			for (; i < data.Length; i++) {
				var seg = data[i];
				if (!IsAsciiLetter(seg[0])) break;
				switch (seg) {
					case ['N', 'D', ..]: EventId = seg[2..]; break;
					case ['N', 'C', 'N', ..]:
						if (seg[3] != '/') SerialType = (JMAEEWSerialType)int.Parse(seg[3].ToString(), CultureInfo.InvariantCulture);
						if (seg[4] != '/') Serial = FromBase36(seg[4]) * 10 + int.Parse(seg[5].ToString(), CultureInfo.InvariantCulture);
						break;
					case ['N', 'C', 'P', 'N', ..]:
						for (i++; i < data.Length && data[i] != "NCP"; i++) ;
						if (i >= data.Length) return;
						break;
				}
			}
			if (data[i][0] != '/') HypocenterCode = data[i];
			i++;
			if (data[i][1] != '/') Latitude = (data[i][0] == 'N' ? 1 : -1) * int.Parse(data[i][1..], CultureInfo.InvariantCulture) / 10f;
			i++;
			if (data[i][1] != '/') Longitude = (data[i][0] == 'E' ? 1 : -1) * int.Parse(data[i][1..], CultureInfo.InvariantCulture) / 10f;
			i++;
			if (data[i][0] != '/') Depth = int.Parse(data[i], CultureInfo.InvariantCulture);
			i++;
			if (data[i][0] != '/') Magnitude = int.Parse(data[i], CultureInfo.InvariantCulture) / 10f;
			i++;
			if (data[i][0] != '/') MaxIntensity = ParseIntensity(data[i]);
			i++;
			for (; i < data.Length; i++) {
				var seg = data[i];
				if (!IsAsciiLetter(seg[0])) break;
				switch (seg) {
					case ['R', 'K', ..]:
						if (seg[2] != '/') HypocenterAccuracy = (JMAEEWHypocenterAccuracy)int.Parse(seg[2].ToString(), CultureInfo.InvariantCulture);
						if (seg[3] != '/') DepthAccuracy = (JMAEEWHypocenterAccuracy)int.Parse(seg[3].ToString(), CultureInfo.InvariantCulture);
						if (seg[4] != '/') MagnitudeAccuracy = (JMAEEWMagnitudeAccuracy)int.Parse(seg[4].ToString(), CultureInfo.InvariantCulture);
						break;
					case ['R', 'T', ..]:
						if (seg[2] != '/') LandOrSea = (JMAEEWLandOrSea)int.Parse(seg[2].ToString(), CultureInfo.InvariantCulture);
						if (seg[3] != '/') ForecastType = (JMAEEWForecastType)int.Parse(seg[3].ToString(), CultureInfo.InvariantCulture);
						if (seg[4] != '/') ForecastMethod = (JMAEEWForecastMethod)int.Parse(seg[4].ToString(), CultureInfo.InvariantCulture);
						break;
					case ['R', 'C', ..]:
						if (seg[2] != '/') MaxIntensityChange = (JMAEEWMaxIntensityChange)int.Parse(seg[2].ToString(), CultureInfo.InvariantCulture);
						if (seg[3] != '/') MaxIntensityChangeReason = (JMAEEWMaxIntensityChangeReason)int.Parse(seg[3].ToString(), CultureInfo.InvariantCulture);
						break;
					case "EBI":
						for (i++; i < data.Length; i++) {
							var seg2 = data[i];
							if (IsAsciiLetter(seg2[0])) break;
							if (seg2 == "9999=") return;
							var code = seg2;
							string? int1 = null;
							string? int2 = null;
							for (i++; i < data.Length; i++) {
								var seg3 = data[i];
								if (!IsAsciiLetter(seg3[0])) break;
								switch (seg3) {
									case ['S', ..]:
										int1 = ParseIntensity(seg3.AsSpan()[1..3]);
										if (seg3[3] != '/') int2 = ParseIntensity(seg3.AsSpan()[3..5]);
										break;
								}
							}
							DateTime? arrivalTime = null;
							if (data[i][0] != '/') arrivalTime = DateTime.ParseExact(data[i], "HHmmss", CultureInfo.InvariantCulture);
							i++;
							JMAEEWForecastType? type = null;
							if (data[i][0] != '/') type = (JMAEEWForecastType)int.Parse(data[i][0].ToString(), CultureInfo.InvariantCulture);
							JMAEEWArrivalCondition? ac = null;
							if (data[i][1] != '/') ac = (JMAEEWArrivalCondition)int.Parse(data[i][1].ToString(), CultureInfo.InvariantCulture);
							m_forecastAreas.Add(new(code, int1, int2, arrivalTime, type, ac));
						}
						break;
				}
			}
		}

		static bool IsAsciiLetter(char v) => v is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z');
		static int FromBase36(char c) => c switch {
			>= '0' and <= '9' => c - '0',
			>= 'A' and <= 'Z' => c - 'A' + 10,
			_ => throw new FormatException("Invalid base-36 character"),
		};
		static string ParseIntensity(ReadOnlySpan<char> v) => v switch {
			"//" => "0",
			"01" => "1",
			"02" => "2",
			"03" => "3",
			"04" => "4",
			"07" => "7",
			_ => v.ToString(),
		};
	}

	public record JMAEEWForecastArea(string Code, string? Intensity1, string? Intensity2, DateTime? ArrivalTime, JMAEEWForecastType? ForecastType, JMAEEWArrivalCondition? ArrivalCondition);

	public enum JMAEEWType {
		None = 0,
		EEWPattern1 = 35,
		EEWPattern2 = 36,
		EEWPattern3 = 37,
		Test = 38,
		Cancellation = 39,
		BroadcastEEW = 47,
		BroadcastEEWCancellation = 48,
		RealtimeIntensity = 61,
	}

	public enum JMAEEWPublishingOffice {
		None = 0,
		Sapporo = 1,
		Sendai = 2,
		Tokyo = 3,
		Osaka = 4,
		Fukuoka = 5,
	}

	public enum JMAEEWStatus {
		General = 0,
		Drilling = 1,
		GeneralCancellation = 10,
		DrillingCancellation = 11,
		ReferenceOrTest = 20,
		AllCodeTest = 30,
	}

	public enum JMAEEWSerialType {
		General = 0,
		Final = 9,
	}

	public enum JMAEEWHypocenterAccuracy {
		Unknown = 0,
		PSWaveLevelExcessionOrIPF1PointOrAssumedHypocenterElement = 1,
		IPF2Points = 2,
		IPF3Or4Points = 3,
		IPFOver5Points = 4,
		NIEDSystemLow = 5,
		NIEDSystemHigh = 6,
		EPOSSea = 7,
		EPOSLand = 8,
	}

	public enum JMAEEWMagnitudeAccuracy {
		Unknown = 0,
		NIEDSystem = 2,
		AllPPhase = 3,
		MixedPhase = 4,
		AllPhase = 5,
		EPOS = 6,
		PSWaveLevelExcessionOrAssumedHypocenterElement = 8,
	}

	public enum JMAEEWLandOrSea {
		Land = 0,
		Sea = 1,
	}

	public enum JMAEEWForecastType {
		Forecast = 0,
		Warning = 1,
	}

	public enum JMAEEWForecastMethod {
		Unknown = 0,
		PLUM = 9,
	}

	public enum JMAEEWMaxIntensityChange {
		MostlyUnchanged = 0,
		Stronger = 1,
		Weaker = 2,
	}

	public enum JMAEEWMaxIntensityChangeReason {
		Unchanged = 0,
		MagnitudeChanged = 1,
		HypocenterChanged = 2,
		MagnitudeAndHypocenterChanged = 3,
		DepthChanged = 4,
		PLUM = 9,
	}

	public enum JMAEEWArrivalCondition {
		NotArrived = 0,
		Arrived = 1,
		NoForecast = 9,
	}
}
