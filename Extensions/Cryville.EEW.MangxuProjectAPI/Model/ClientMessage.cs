using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cryville.EEW.MangxuProjectAPI.Model {
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
	[JsonDerivedType(typeof(SubscribeMessage), "subscribe")]
	public record ClientMessage;

	public record SubscribeMessage(
		float? MinMag,
		IList<string>? Agencies
	) : ClientMessage;
}
