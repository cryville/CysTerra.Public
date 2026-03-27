using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cryville.EEW.JMA {
	[JsonSerializable(typeof(Dictionary<string, string[]>))]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
