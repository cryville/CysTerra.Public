using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.GlobalQuake.Features {
	public class GlobalQuakeReportFeatureGenerator : IGenerator<GlobalQuakeReport, Feature?> {
		public Feature? Generate(GlobalQuakeReport e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			if (e.RevisionId == -1) {
				return null;
			}
			var q = e.Quality as HypocenterQualityData;
			return new Feature {
				{ Is, Earthquake },
				{ Ongoing, !e.IsArchive },
				{ Time, new DateTimeOffset(e.OriginTime, TimeSpan.Zero) },
				{ TimeModified, new DateTimeOffset(e.LastUpdatedTime, TimeSpan.Zero) },
				{ At, new Feature(new Point(e.Longitude, e.Latitude)) {
					{ Is, Hypocenter },
					{ HypocenterDepth, q != null ? new QuantityUnc(e.Depth, q.ErrorDepth, DerivedMeasures.Kilometre) : new Quantity(e.Depth, DerivedMeasures.Kilometre) },
				} },
				{ Magnitude, new Quantity(e.Magnitude, Units.Dimensionless) },
			};
		}
	}
}
