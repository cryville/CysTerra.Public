using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
	[JsonDerivedType(typeof(FANStudioHeartbeatMessage), "heartbeat")]
	[JsonDerivedType(typeof(FANStudioDataMessage), "initial")]
	[JsonDerivedType(typeof(FANStudioInitialAllMessage), "initial_all")]
	[JsonDerivedType(typeof(FANStudioUpdateMessage), "update")]
	public record FANStudioMessage;

	public record FANStudioHeartbeatMessage(
		[property: JsonPropertyName("ver")] string Version,
		[property: JsonPropertyName("id")] Guid ID,
		[property: JsonPropertyName("timestamp")] long Timestamp
	) : FANStudioMessage;

	public record FANStudioDataMessage(
		JsonElement Data,
		[property: JsonPropertyName("md5")] string MD5
	) : FANStudioMessage;

	public interface IFANStudioData<out T> {
		T Data { get; }
		string MD5 { get; }
		string? Source { get; }
	}
	public record FANStudioData<T>(
		T Data,
		[property: JsonPropertyName("md5")] string MD5,
		[property: JsonPropertyName("source")] string Source
	) : IFANStudioData<T>;

	public record FANStudioInitialAllMessage(
		[property: JsonPropertyName("bcsf")] FANStudioData<BCSFEarthquake>? BCSFEarthquake,
		[property: JsonPropertyName("beijing")] FANStudioData<BeijingEarthquake>? BeijingEarthquake,
		[property: JsonPropertyName("cea")] FANStudioData<CEAEEW>? CEAEEW,
		[property: JsonPropertyName("cea-pr")] FANStudioData<CEAProvinceEEW>? CEAProvinceEEW,
		[property: JsonPropertyName("cenc")] FANStudioData<CENCEarthquake>? CENCEarthquake,
		[property: JsonPropertyName("cwa-eew")] FANStudioData<CWAEEW>? CWAEEW,
		[property: JsonPropertyName("emsc")] FANStudioData<EMSCEarthquake>? EMSCEarthquake,
		[property: JsonPropertyName("fssn")] FANStudioData<FSSNEarthquake>? FSSNEarthquake,
		[property: JsonPropertyName("fujian")] FANStudioData<FujianEEW>? FujianEEW,
		[property: JsonPropertyName("gfz")] FANStudioData<GFZEarthquake>? GFZEarthquake,
		[property: JsonPropertyName("guangxi")] FANStudioData<GuangxiEarthquake>? GuangxiEarthquake,
		[property: JsonPropertyName("hko")] FANStudioData<HKOEarthquake>? HKOEarthquake,
		[property: JsonPropertyName("icl")] FANStudioData<ICLEEW>? ICLEEW,
		[property: JsonPropertyName("kma")] FANStudioData<KMAEarthquake>? KMAEarthquake,
		[property: JsonPropertyName("ningxia")] FANStudioData<NingxiaEarthquake>? NingxiaEarthquake,
		[property: JsonPropertyName("sa")] FANStudioData<ShakeAlertEEW>? ShakeAlertEEW,
		[property: JsonPropertyName("shanxi")] FANStudioData<ShanxiEarthquake>? ShanxiEarthquake,
		[property: JsonPropertyName("sichuan")] FANStudioData<SichuanEEW>? SichuanEEW,
		[property: JsonPropertyName("tsunami")] FANStudioData<NMEFCTsunamiWarning>? NMEFCTsunamiWarning,
		[property: JsonPropertyName("usgs")] FANStudioData<USGSEarthquake>? USGSEarthquake,
		[property: JsonPropertyName("usp")] FANStudioData<USPEarthquake>? USPEarthquake,
		[property: JsonPropertyName("weatheralarm")] FANStudioData<CMAWeatherAlarm>? CMAWeatherAlarm
	) : FANStudioMessage {
		public IEnumerable<IFANStudioData<object>> Enumerate() {
			if (BCSFEarthquake != null)
				yield return BCSFEarthquake;
			if (BeijingEarthquake != null)
				yield return BeijingEarthquake;
			if (CEAEEW != null)
				yield return CEAEEW;
			if (CEAProvinceEEW != null)
				yield return CEAProvinceEEW;
			if (CENCEarthquake != null)
				yield return CENCEarthquake;
			if (CWAEEW != null)
				yield return CWAEEW;
			if (CMAWeatherAlarm != null)
				yield return CMAWeatherAlarm;
			if (EMSCEarthquake != null)
				yield return EMSCEarthquake;
			if (FSSNEarthquake != null)
				yield return FSSNEarthquake;
			if (FujianEEW != null)
				yield return FujianEEW;
			if (GFZEarthquake != null)
				yield return GFZEarthquake;
			if (GuangxiEarthquake != null)
				yield return GuangxiEarthquake;
			if (HKOEarthquake != null)
				yield return HKOEarthquake;
			if (ICLEEW != null)
				yield return ICLEEW;
			if (KMAEarthquake != null)
				yield return KMAEarthquake;
			if (NingxiaEarthquake != null)
				yield return NingxiaEarthquake;
			if (NMEFCTsunamiWarning != null)
				yield return NMEFCTsunamiWarning;
			if (ShakeAlertEEW != null)
				yield return ShakeAlertEEW;
			if (ShanxiEarthquake != null)
				yield return ShanxiEarthquake;
			if (SichuanEEW != null)
				yield return SichuanEEW;
			if (USGSEarthquake != null)
				yield return USGSEarthquake;
			if (USPEarthquake != null)
				yield return USPEarthquake;
		}
	}

	public record FANStudioUpdateMessage(
		JsonElement Data,
		string MD5,
		[property: JsonPropertyName("source")] string Source
	) : FANStudioDataMessage(Data, MD5);
}
