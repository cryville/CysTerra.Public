using Cryville.Common.Compat;
using Cryville.EEW.Report;
using QuakeML;
using System;
using System.Globalization;

namespace Cryville.EEW.QuakeML {
	public class QuakeMLEventReportGenerator : IContextedGenerator<Event, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(Event e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			var result = new ReportModel {
				Title = res.GetStringRequired("Title"),
			};

			if (e.creationInfo?.agencyID is string agency)
				if (result.Source == null) result.Source = agency;
				else result.Source += " | " + agency;
			else if (e.creationInfo?.author is string author)
				if (result.Source == null) result.Source = author;
				else result.Source += " | " + author;

			Origin? mainOrigin = null;
			if (e.origin is Origin[] origins && origins.Length > 0) {
				mainOrigin = origins[0];
				GenerateLocationFromOrigin(context, res, culture, result, mainOrigin, origins.Length);
				result.Time = new(mainOrigin.time.value, TimeSpan.Zero);
				result.TimeZone = TimeZoneInfo.Utc;
			}

			if (result.Location == null && e.description is EventDescription[] descs) {
				foreach (var desc in descs) {
					if (desc.typeSpecified && desc.type is not (EventDescriptionType.FlinnEngdahlregion or EventDescriptionType.regionname))
						continue;
					result.Location = desc.text;
					result.LocationSpecificity = desc.type is EventDescriptionType.FlinnEngdahlregion ? 3 : 0;
					break;
				}
			}

			result.Predicate = res.GetStringSet("EventType")?.GetString(e.type.ToString());
			if (result.Predicate != null && e.typeCertainty == EventTypeCertainty.suspected) {
				result.Predicate = string.Format(SharedCultures.CurrentCulture, res.GetStringRequired("EventTypeSuspected"), result.Predicate);
			}

			Magnitude? mainMagnitude = null;
			if (e.magnitude is Magnitude[] magnitudes) {
				foreach (var mag in magnitudes) {
					mainMagnitude ??= mag;
					result.Properties.Add(new(mag.type.ToUpperInvariant() switch {
						['M', 'B', ..] => TagTypeKeys.MagnitudeBodyWave,
						"MC" or "MD" => TagTypeKeys.MagnitudeDuration,
						['M', 'L', ..] => TagTypeKeys.MagnitudeLocal,
						"MS" => TagTypeKeys.MagnitudeSurfaceWave,
						['M', 'W', ..] => TagTypeKeys.MagnitudeMoment,
						_ => TagTypeKeys.Magnitude,
					}, mag.type, FormatRealQuantity(mag.mag, culture), context.SeverityScheme, mag.mag.value) {
						AccuracyOrder = GetAccuracyOrder(
							mag.evaluationModeSpecified, mag.evaluationMode,
							mag.evaluationStatusSpecified, mag.evaluationStatus
						)
					});
				}
			}

			if (mainOrigin != null) {
				double? depthValue = null;
				if (mainOrigin.depth is RealQuantity depth) {
					result.Properties.Add(new(
						TagTypeKeys.HypocenterDepth,
						res.GetStringRequired("PropertyDepth"),
						string.Format(culture, res.GetStringRequired("PropertyDepthValue"), FormatRealQuantity(depth, culture, 1e-3)),
						context.SeverityScheme,
						depthValue = depth.value / 1000
					) {
						AccuracyOrder = GetAccuracyOrder(
							mainOrigin.evaluationModeSpecified, mainOrigin.evaluationMode,
							mainOrigin.evaluationStatusSpecified, mainOrigin.evaluationStatus
						)
					});
				}
				result.GroupKeys.Add(new HypocenterGroupKey(mainOrigin.latitude.value, mainOrigin.longitude.value, mainOrigin.time.value, mainMagnitude?.mag.value ?? 1, depthValue));
			}

			result.GroupKeys.Add(new ReportUnitKey(e.publicID));
			if (e.creationInfo is CreationInfo creationInfo) {
				result.RevisionKey = new ReportRevisionKey(creationInfo.creationTimeSpecified ? creationInfo.creationTime : null);
			}

			ApplyExtensions(e, ref result, context, culture);
			return result;
		}
		static void ApplyExtensions(Event e, ref ReportModel result, IReportGeneratorContext context, CultureInfo culture) {
			if (QuakeMLExtensionCollector.Instance is not { } collector || collector.Components.Count == 0)
				return;
			foreach (var ext in collector.Components) {
				if (ext is IContextedGenerator<(Event, ReportModel), IReportGeneratorContext, ReportModel> cext) {
					result = cext.Generate((e, result), context, ref culture);
				}
				else {
					result = ext.Generate((e, result), ref culture);
				}
			}
		}

		static void GenerateLocationFromOrigin(IReportGeneratorContext context, IMessageStringSet res, CultureInfo culture, ReportModel result, Origin mainOrigin, int originCount) {
			if (!context.NameLocationTo(result, mainOrigin.latitude.value, mainOrigin.longitude.value, null, culture)) {
				if (mainOrigin.region is string region) {
					result.Location = region;
					result.LocationSpecificity = 0;
				}
			}
			if (result.Location == null) return;
			if (originCount > 1) {
				result.Location += string.Format(culture, res.GetStringRequired("LocationAggregated"), result.Location, originCount - 1);
			}
		}

		static string FormatRealQuantity(RealQuantity quantity, CultureInfo culture, double multiplier = 1) {
			double value = quantity.value * multiplier;
			double uncertainty;
			if (quantity.uncertaintySpecified) uncertainty = quantity.uncertainty;
			else if (quantity.upperUncertaintySpecified)
				if (quantity.lowerUncertaintySpecified) uncertainty = Math.Max(quantity.upperUncertainty, quantity.lowerUncertainty);
				else uncertainty = quantity.upperUncertainty;
			else if (quantity.lowerUncertaintySpecified) uncertainty = quantity.lowerUncertainty;
			else return value.ToString(culture);
			uncertainty *= multiplier;
			if (uncertainty <= 0)
				return value.ToString(culture);
			return value.ToString("F" + Math.Max(0, (int)Math.Ceiling(-Math.Log10(2 * uncertainty))), culture);
		}

		static int GetAccuracyOrder(bool evaluationModeSpecified, EvaluationMode evaluationMode, bool evaluationStatusSpecified, EvaluationStatus evaluationStatus) =>
			(evaluationModeSpecified ? evaluationMode switch {
				EvaluationMode.manual => 10,
				_ => 30,
			} : 30) +
			(evaluationStatusSpecified ? evaluationStatus switch {
				EvaluationStatus.rejected => 5,
				EvaluationStatus.confirmed => -2,
				EvaluationStatus.reviewed => -5,
				EvaluationStatus.final => -8,
				_ => 0,
			} : 0);

		sealed record ReportUnitKey(string PublicID) : IReportUnitKey;
		sealed record ReportRevisionKey(DateTime? CreationTime) : IReportRevisionKey {
			public bool IsComparableWith(IReportRevisionKey obj) => obj is ReportRevisionKey;
			public int CompareTo(IReportRevisionKey? obj) {
				if (obj is not ReportRevisionKey other) throw new ArgumentException("Mismatched revision key type.");
				if (CreationTime is DateTime aTime && other.CreationTime is DateTime bTime) return aTime.CompareTo(bTime);
				return 0;
			}
		}
	}
}
