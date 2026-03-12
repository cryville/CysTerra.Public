using System;

namespace Cryville.EEW.GlobalQuake {
	public record GlobalQuakeReport(
		float Latitude,
		float Longitude,
		float Depth,
		float Magnitude,
		DateTime LastUpdatedTime,
		DateTime OriginTime,
		int RevisionId,
		string? Region,
		Guid Id,
		IHypocenterQualityData? Quality,
		bool IsArchive = false
	);

	public interface IHypocenterQualityData {
		int QualityLevel { get; }
	}

	public record HypocenterQualityClass(int QualityId) : IHypocenterQualityData {
		public int QualityLevel => QualityId;
	}

	public record HypocenterQualityData(
		float ErrorDepth,
		float ErrorEW,
		float ErrorNS,
		float ErrorOrigin,
		float Percentage,
		int StationCount
	) : IHypocenterQualityData {
		public int QualityLevel =>
			Math.Max(ErrorDepth switch {
				< 8 => 0,
				< 20 => 1,
				< 40 => 2,
				< 100 => 3,
				_ => 4,
			}, Math.Max(ErrorEW switch {
				< 5 => 0,
				< 12 => 1,
				< 24 => 2,
				< 60 => 3,
				_ => 4,
			}, Math.Max(ErrorNS switch {
				< 5 => 0,
				< 12 => 1,
				< 24 => 2,
				< 60 => 3,
				_ => 4,
			}, Math.Max(ErrorOrigin switch {
				< 1.2f => 0,
				< 3 => 1,
				< 9 => 2,
				< 20 => 3,
				_ => 4,
			}, Math.Max(Percentage switch {
				>= 90 => 0,
				>= 80 => 1,
				>= 65 => 2,
				>= 55 => 3,
				_ => 4,
			}, StationCount switch {
				>= 12 => 0,
				>= 10 => 1,
				>= 8 => 2,
				>= 6 => 3,
				_ => 4,
			})))));
	}
}
