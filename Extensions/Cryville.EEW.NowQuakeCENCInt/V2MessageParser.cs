using Cryville.Common.Compat;
using Cryville.EEW.NowQuakeCENCInt.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Cryville.EEW.NowQuakeCENCInt {
	public static class V2MessageParser {
		static readonly Dictionary<Schema, Func<V2Message, object>> _types = new(SchemaEqualityComparer.Instance) {
			{ new() { Type = "cenc-intensity-report", Version = "1.0.0" }, m => ParseCore(m, V2SerializerContext.Default.V2Report) },
			{ new() { Type = "cenc-intensity-stream", Version = "1.0.0" }, m => {
				var metadataBase = m.Metadata.Deserialize(V2SerializerContext.Default.V2StreamMetadata) ?? throw new JsonException("Null metadata.");
				return metadataBase.Type switch {
					"heartbeat" => ParseCore(m, V2SerializerContext.Default.V2Heartbeat),
					"pong" => ParseCore(m, V2SerializerContext.Default.V2Pong),
					_ => throw new NotSupportedException("Unsupported stream message."),
				};
			} },
			{ new() { Type = "cenc-intensity-report-collection", Version = "1.0.0" }, m => ParseCore(m, V2SerializerContext.Default.V2Collection) },
			{ new() { Type = "cenc-intensity-error", Version = "1.0.0" }, m => ParseCore(m, V2SerializerContext.Default.V2SocketError) },
		};
		static object ParseCore(V2Message message, JsonTypeInfo typeInfo) {
#pragma warning disable IDE0079
#pragma warning disable IL2072
			var obj = Activator.CreateInstance(typeInfo.Type)!;
#pragma warning restore IL2072
#pragma warning restore IDE0079
			foreach (var prop in typeInfo.Properties) {
				var name = prop.Name;
				if (name == "schema")
					prop.Set?.Invoke(obj, message.Schema);
				else {
					JsonElement? d = name switch {
						"metadata" => message.Metadata,
						"data" => message.Data,
						_ => null,
					};
					if (d is JsonElement element) {
						var propTypeInfo = V2SerializerContext.Default.GetTypeInfo(prop.PropertyType) ?? throw new JsonException("Missing type info for metadata.");
						prop.Set?.Invoke(obj, element.Deserialize(propTypeInfo) ?? throw new JsonException("Null metadata or data."));
					}
				}
			}
			return obj;
		}

		public static object Parse(V2Message message) {
			ThrowHelper.ThrowIfNull(message);
			if (!_types.TryGetValue(message.Schema, out var typeInfo))
				throw new NotSupportedException("Unsupported schema.");
			return typeInfo(message);
		}

		sealed class SchemaEqualityComparer : IEqualityComparer<Schema> {
			public static SchemaEqualityComparer Instance => field ??= new();

			public bool Equals(Schema? x, Schema? y) {
				if (x is null)
					return y is null;
				if (y is null)
					return false;
				return x.Type == y.Type && x.Version == y.Version;
			}
			public int GetHashCode(Schema obj) => HashCode.Combine(obj.Type, obj.Version);
		}
	}
}
