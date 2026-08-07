using Cryville.Common.Compat;
using Cryville.EEW.Report;
using Cryville.EEW.USGS.Model;
using System;
using System.Globalization;

namespace Cryville.EEW.USGS {
	public sealed class USGSContoursReportGenerator : IContextedGenerator<USGSContours, IReportGeneratorContext, ReportModel> {
		public ReportModel Generate(USGSContours e, IReportGeneratorContext? context, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);
			context ??= EmptyReportGeneratorContext.Instance;

			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			string name = USGSHelpers.GetOrInferName(e);
			var result = new ReportModel {
				Title = string.Format(culture, res.GetStringRequired("ShakeMapTitle"), res.GetStringSetRequired("ContoursName").GetStringOrDefault(name)),
				Source = res.GetStringRequired("AuthorityName"),
			};
			var metadata = e.Metadata;
			context.NameLocationTo(result, metadata.Latitude, metadata.Longitude, null, culture);

			result.GroupKeys.Add(new USGSEventIDGroupKey(metadata.EventID));

			bool hasMaxProp = false;
			var contourPropType = USGSHelpers.GetPropertyType(name);
			if (e is USGSContoursProduct ep) {
				GenerateFromProduct(context, culture, res, result, ref hasMaxProp, ep, contourPropType, name);
			}

			return result;
		}

		static void GenerateFromProduct(IReportGeneratorContext context, CultureInfo culture, IMessageStringSet res, ReportModel result, ref bool hasMaxProp, USGSContoursProduct ep, TagTypeKey contourPropType, string name) {
			var product = ep.Product;
			USGSHelpers.ApplyCommonProductInfo(result, product, ep.FileName, ep.Content, res, culture);
			var props = product.Properties;
			if (props == null) return;
			if (
				props.TryGetValue("eventtime", out string? eventtimeValue) &&
				DateTimeOffset.TryParse(eventtimeValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var eventtime)
			) {
				result.Time = eventtime;
				result.TimeZone = TimeZoneInfo.Utc;
			}
			string maxProp = name switch {
				"cont_mi" or "cont_mmi" => "maxmmi",
				"cont_pga" => "maxpga",
				"cont_pgv" => "maxpgv",
				_ => throw new NotSupportedException(),
			};

			static bool TryParseSingleMaybeNaN(string s, out float value) {
				if (float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
					return true;
				// Fix for NaN check being case-sensitive prior to .NET Core 3.0
				if (s.Equals(CultureInfo.InvariantCulture.NumberFormat.NaNSymbol, StringComparison.OrdinalIgnoreCase)) {
					value = float.NaN;
					return true;
				}
				return false;
			}

			if (
				props.TryGetValue(maxProp, out string? maxPropValue) &&
				TryParseSingleMaybeNaN(maxPropValue, out float maxValue)
			) {
				hasMaxProp = true;
				string unit = maxProp switch {
					"maxmmi" => "mmi",
					"maxpga" => "g", // Not documented, assuming here
					"maxpgv" => "cms",
					_ => throw new NotSupportedException(),
				};
				if (float.IsNaN(maxValue))
					result.Properties.Add(new(
						contourPropType,
						res.GetStringSetRequired("ContoursPropertyName").GetStringOrDefault(name),
						res.GetStringRequired("PropertyValueUnknown"),
						-1
					) { AccuracyOrder = 50 });
				else if (contourPropType == TagTypeKeys.IntensityMMI)
					result.Properties.Add(RomanNumerals.CreateRomanIntensityProperty(
						contourPropType,
						res.GetStringSetRequired("ContoursPropertyName").GetStringOrDefault(name),
						maxValue,
						culture,
						context.SeverityScheme,
						50
					));
				else
					result.Properties.Add(new(
						contourPropType,
						res.GetStringSetRequired("ContoursPropertyName").GetStringOrDefault(name),
						string.Format(culture, res.GetStringRequired("PropertyValue"), FormatQuantity(maxValue, culture, 2), res.GetStringSetRequired("PropertyUnits").GetString(unit)),
						context.SeverityScheme,
						USGSHelpers.ExtractPropertyValue(maxValue, unit)
					) { AccuracyOrder = 50 });
			}
			double? magnitudeValue = null;
			if (
				props.TryGetValue("magnitude", out string? magnitudeString) &&
				float.TryParse(magnitudeString, NumberStyles.Any, CultureInfo.InvariantCulture, out var magnitude)
			) {
				magnitudeValue = magnitude;
				result.Properties.Add(new(
					TagTypeKeys.Magnitude,
					res.GetStringRequired("PropertyMagnitude"),
					magnitude.ToString("F1", culture),
					context.SeverityScheme,
					magnitude
				) { AccuracyOrder = 10 });
			}
			double? depthValue = null;
			if (
				props.TryGetValue("depth", out string? depthString) &&
				float.TryParse(depthString, NumberStyles.Any, CultureInfo.InvariantCulture, out var depth)
			) {
				depthValue = depth;
				result.Properties.Add(new(
					TagTypeKeys.HypocenterDepth,
					res.GetStringRequired("PropertyDepth"),
					string.Format(culture, res.GetStringRequired("PropertyDepthValue"), depth.ToString("F1", culture)),
					context.SeverityScheme,
					depth
				) { AccuracyOrder = 10 });
			}
			if (result.Time is DateTimeOffset time && magnitudeValue is double magnitudeValue2) {
				result.GroupKeys.Add(new HypocenterGroupKey(ep.Metadata.Latitude, ep.Metadata.Longitude, time.UtcDateTime, magnitudeValue2, depthValue));
			}
		}

		static string FormatQuantity(float value, CultureInfo culture, int minSigDigits) {
			return value.ToString("F" + Math.Max(0, (int)Math.Ceiling(-Math.Log10(value)) + minSigDigits - 1), culture);
		}
	}
}
