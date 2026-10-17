using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cryville.EEW.MangxuProjectAPI.Model {
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = true)]
	[JsonDerivedType(typeof(FullSnapshotMessage), "full_snapshot")]
	[JsonDerivedType(typeof(SingleAgencyUpdateMessage), "single_agency_update")]
	[JsonDerivedType(typeof(BatchAgencyUpdateMessage), "batch_agency_update")]
	public record Message(
		DateTime TimestampUtc,
		bool FilterActive
	);

	public record FullSnapshotMessage(
		DateTime TimestampUtc,
		string Channel,
		int TotalEvents,
		int TotalEventsUnfiltered,
		IDictionary<string, string> AgenciesStatus,
		bool FilterActive,
		IList<EarthquakeEvent> Earthquakes
	) : Message(TimestampUtc, FilterActive);

	public record SingleAgencyUpdateMessage(
		DateTime TimestampUtc,
		string AgencyId,
		string AgencyName,
		bool FilterActive,
		int NewCount,
		IList<EarthquakeEvent> Events
	) : Message(TimestampUtc, FilterActive);

	public record BatchAgencyUpdateMessage(
		DateTime TimestampUtc,
		bool FilterActive,
		int TotalAgenciesUpdated,
		int TotalNewEvents,
		IList<SingleAgencyUpdate> Updates
	) : Message(TimestampUtc, FilterActive);

	public record SingleAgencyUpdate(
		string AgencyId,
		string AgencyName,
		int NewCount,
		IList<EarthquakeEvent> Events
	);
}
