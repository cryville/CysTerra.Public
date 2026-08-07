using Cryville.Common.Compat;
using Cryville.EEW.Features;
using Cryville.Measure;
using QuakeML;
using System;
using System.Collections.Generic;
using System.Globalization;
using static Cryville.EEW.TagTypeKeys;
using Tag = System.Collections.Generic.KeyValuePair<Cryville.EEW.TagTypeKey, object?>;

namespace Cryville.EEW.QuakeML.Features {
	public class QuakeMLEventFeatureGenerator : IGenerator<Event, Feature> {
		public Feature Generate(Event e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			List<Feature>? fSubjects = null;
			var f = new Feature {
				{ Is, ReportObservation },
				//{ Time, new DateTimeOffset(e., TimeSpan.Zero) },
				//{ TimeModified, new DateTimeOffset(e.LastUpdatedTime, TimeSpan.Zero) },
				//{ At, new Feature(new Point(e.Longitude, e.Latitude)) {
				//	{ Is, Hypocenter },
				//	{ HypocenterDepth, q != null ? new QuantityUnc(e.Depth, q.ErrorDepth, DerivedMeasures.Kilometre) : new Quantity(e.Depth, DerivedMeasures.Kilometre) },
				//} },
				//{ Magnitude, new Quantity(e.Magnitude, Units.Dimensionless) },
			};
			var magnitudes = CollectMagnitudes(e.magnitude);
			if (e.origin is { } origins && origins.Length > 0) {
				fSubjects = [];
				foreach (var origin in origins) {
					fSubjects.Add(GenerateFromOrigin(e, origin, magnitudes));
				}
				f.Add(Subject, fSubjects);
			}
			foreach (var (_, magnitude) in magnitudes) {
				f.TryAdd(magnitude.Key, magnitude.Value);
			}
			ApplyComment(f, e.comment);
			ApplyCreationInfo(f, e.creationInfo);
			return f;
		}

		static Dictionary<string, Tag> CollectMagnitudes(Magnitude[] magnitudes) {
			var result = new Dictionary<string, Tag>(magnitudes.Length);
			foreach (var magnitude in magnitudes) {
				string? originID = magnitude.originID;
				if (string.IsNullOrWhiteSpace(originID))
					originID = "guid:" + Guid.NewGuid().ToString(null, CultureInfo.InvariantCulture);
				result.Add(magnitude.originID, new(magnitude.type.ToUpperInvariant() switch {
					['M', 'B', ..] => MagnitudeBodyWave,
					"MC" or "MD" => MagnitudeDuration,
					['M', 'L', ..] => MagnitudeLocal,
					"MS" => MagnitudeSurfaceWave,
					['M', 'W', ..] => MagnitudeMoment,
					_ => TagTypeKeys.Magnitude,
				}, ConvertQuantity(magnitude.mag, Units.Dimensionless)));
			}
			return result;
		}

		static Feature GenerateFromOrigin(Event e, Origin origin, Dictionary<string, Tag> magnitudes) {
			var f = new Feature {
				{ Is, Earthquake },
				{ Time, new DateTimeOffset(origin.time.value, TimeSpan.Zero) },
			};
			string? name = null;
			string? description = null;
			string? regionNameOverride = null;
			if (e.description is EventDescription[] descs) {
				foreach (var desc in descs) {
					if (desc.typeSpecified) {
						if (desc.type is EventDescriptionType.earthquakename) {
							name ??= desc.text;
							continue;
						}
						if (desc.type is EventDescriptionType.FlinnEngdahlregion or EventDescriptionType.regionname) {
							regionNameOverride ??= desc.text;
							continue;
						}
					}
					description ??= desc.text;
				}
			}
			if (name != null)
				f.Add(Name, name);
			if (description != null)
				f.Add(Description, description);
			f.Add(At, GenerateHypocenterFromOrigin(origin, regionNameOverride));
			if (magnitudes.Remove(origin.publicID, out var magTag))
				f.Add(magTag);
			ApplyComment(f, origin.comment);
			ApplyCreationInfo(f, origin.creationInfo);
			return f;
		}
		static Feature GenerateHypocenterFromOrigin(Origin origin, string? regionNameOverride) {
			var f = new Feature(new Point(origin.longitude.value, origin.latitude.value)) {
				{ Is, Hypocenter },
			};
			if (origin.depth is { } depth)
				f.Add(HypocenterDepth, ConvertQuantity(depth, Units.Metre));
			if (regionNameOverride != null)
				f.Add(Name, regionNameOverride);
			else if (origin.region is { } region)
				f.Add(Name, region);
			return f;
		}

		static void ApplyComment(Feature f, Comment[]? comment) {
			if (comment == null || comment.Length == 0)
				return;
			List<string> comments = [];
			foreach (var c in comment)
				comments.Add(c.text); // TODO Comment feature
			f.Add(TagTypeKeys.Comment, comments);
		}
		static void ApplyCreationInfo(Feature f, CreationInfo? creationInfo) {
			if (creationInfo == null)
				return;
			if (creationInfo.agencyID is { } agencyID)
				f.Add(Source, agencyID);
			else if (creationInfo.author is { } author)
				f.Add(Source, author);
			if (creationInfo.creationTime is { } creationTime)
				f.Add(TimeModified, new DateTimeOffset(creationTime, TimeSpan.Zero));
		}

		static object ConvertQuantity(RealQuantity quantity, Unit unit) {
			if (quantity.lowerUncertaintySpecified) {
				if (quantity.upperUncertaintySpecified)
					return new QuantityUnc(quantity.value, quantity.lowerUncertainty, quantity.upperUncertainty, unit);
				return new QuantityUnc(quantity.value, quantity.lowerUncertainty, 0, unit);
			}
			if (quantity.upperUncertaintySpecified)
				return new QuantityUnc(quantity.value, 0, quantity.upperUncertainty, unit);
			if (quantity.uncertaintySpecified)
				return new QuantityUnc(quantity.value, quantity.uncertainty, unit);
			return new Quantity(quantity.value, unit);
		}
	}
}
