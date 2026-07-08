using System.Text.Json.Serialization;

namespace Cryville.EEW.Wolfx.Model {
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
	[JsonDerivedType(typeof(Heartbeat), "heartbeat")]
	[JsonDerivedType(typeof(SichuanEEW), "sc_eew")]
	[JsonDerivedType(typeof(FujianEEW), "fj_eew")]
	[JsonDerivedType(typeof(ChongqingEEW), "cq_eew")]
	[JsonDerivedType(typeof(CENCEEW), "cenc_eew")]
	[JsonDerivedType(typeof(CWAEEW), "cwa_eew")]
	[JsonDerivedType(typeof(JMAEEW), "jma_eew")]
	[JsonDerivedType(typeof(WolfxEarthquakeList<JMAEarthquake>), "jma_eqlist")]
	[JsonDerivedType(typeof(WolfxEarthquakeList<CENCEarthquake>), "cenc_eqlist")]
	public record BaseModel { }
}
