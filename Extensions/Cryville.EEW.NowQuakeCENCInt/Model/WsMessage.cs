using System;
using System.Text.Json.Serialization;

namespace Cryville.EEW.NowQuakeCENCInt.Model {
	[Obsolete("Use the V2 protocol instead.")]
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
	[JsonDerivedType(typeof(WsHeartbeat), "heartbeat")]
	[JsonDerivedType(typeof(WsData), "data")]
	[JsonDerivedType(typeof(WsError), "error")]
	public class WsMessage;
}
