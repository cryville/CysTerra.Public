using Cryville.EEW.FANStudio.Model;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio {
	[JsonSerializable(typeof(FANStudioMessage))]
	[JsonSerializable(typeof(FANStudioData<CMAWeatherAlarm>))]
	[JsonSerializable(typeof(FANStudioData<NMEFCTsunamiWarning>))]
	[JsonSerializable(typeof(FANStudioData<CENCEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<CEAEEW>))]
	[JsonSerializable(typeof(FANStudioData<CEAProvinceEEW>))]
	[JsonSerializable(typeof(FANStudioData<NingxiaEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<GuangxiEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<ShanxiEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<BeijingEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<CWAEEW>))]
	[JsonSerializable(typeof(FANStudioData<HKOEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<USGSEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<ShakeAlertEEW>))]
	[JsonSerializable(typeof(FANStudioData<EMSCEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<BCSFEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<GFZEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<USPEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<KMAEarthquake>))]
	[JsonSerializable(typeof(FANStudioData<FSSNEarthquake>))]
	[JsonSourceGenerationOptions(Converters = [typeof(NonstandardDateTimeJsonConverter)], AllowOutOfOrderMetadataProperties = true)]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
