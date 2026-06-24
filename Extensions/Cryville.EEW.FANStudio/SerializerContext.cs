using Cryville.EEW.FANStudio.Model;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio {
	[JsonSerializable(typeof(FANStudioMessage))]
	[JsonSourceGenerationOptions(Converters = [typeof(NonstandardDateTimeJsonConverter)], AllowOutOfOrderMetadataProperties = true)]
	sealed partial class SerializerContext : JsonSerializerContext { }
}
