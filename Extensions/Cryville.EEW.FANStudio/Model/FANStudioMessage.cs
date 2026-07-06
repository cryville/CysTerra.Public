using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cryville.EEW.FANStudio.Model {
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
	[JsonDerivedType(typeof(FANStudioHeartbeatMessage), "heartbeat")]
	[JsonDerivedType(typeof(FANStudioAuthRequiredMessage), "auth_required")]
	[JsonDerivedType(typeof(FANStudioAuthSuccessMessage), "auth_success")]
	[JsonDerivedType(typeof(FANStudioAuthFailureMessage), "auth_fail")]
	[JsonDerivedType(typeof(FANStudioErrorMessage), "error")]
	[JsonDerivedType(typeof(FANStudioDataMessage), "initial")]
	[JsonDerivedType(typeof(FANStudioInitialAllMessage), "initial_all")]
	[JsonDerivedType(typeof(FANStudioUpdateMessage), "update")]
	public record FANStudioMessage;

	public record FANStudioHeartbeatMessage(
		[property: JsonPropertyName("ver")] string Version,
		[property: JsonPropertyName("id")] Guid ID,
		[property: JsonPropertyName("timestamp")] long Timestamp
	) : FANStudioMessage;

	public record FANStudioSimpleMessage(
		[property: JsonPropertyName("message")] string Message
	) : FANStudioMessage;
	public record FANStudioAuthRequiredMessage(string Message) : FANStudioSimpleMessage(Message);
	public record FANStudioAuthSuccessMessage(string Message) : FANStudioSimpleMessage(Message);
	public record FANStudioAuthFailureMessage(string Message) : FANStudioSimpleMessage(Message);
	public record FANStudioErrorMessage(string Message) : FANStudioSimpleMessage(Message);

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

	public record FANStudioInitialAllMessage : FANStudioMessage {
		[JsonExtensionData]
		[SuppressMessage("CodeQuality", "IDE0079", Justification = "False report")]
		[SuppressMessage("Usage", "CA2227", Justification = "DTO")]
		public IDictionary<string, JsonElement>? Data { get; set; }
	}

	public record FANStudioUpdateMessage(
		JsonElement Data,
		string MD5,
		[property: JsonPropertyName("source")] string Source
	) : FANStudioDataMessage(Data, MD5);
}
