using Cryville.Common.Compat;
using Cryville.EEW.Report;
using Cryville.EEW.USGS.Model;
using System;
using System.Globalization;
using System.Linq;

namespace Cryville.EEW.USGS {
	public static class USGSHelpers {
		public static string GetOrInferName(USGSContours e) {
			ThrowHelper.ThrowIfNull(e);
			if (e.Name is string name) {
				return name;
			}
			if (e is USGSContoursProduct ep) {
				string fileName = ep.FileName;
				if (fileName.StartsWith("download/", StringComparison.Ordinal) && fileName.EndsWith(".json", StringComparison.Ordinal)) {
					return fileName[9..^5];
				}
			}
			return e.Features.First().Properties.Units switch {
				"mmi" => "cont_mmi",
				"pctg" => "cont_pga",
				"cms" => "cont_pgv",
				_ => throw new NotSupportedException(),
			};
		}
		public static TagTypeKey GetPropertyType(string name) {
			return name switch {
				"cont_mi" or "cont_mmi" => TagTypeKeys.IntensityMMI,
				"cont_pga" => TagTypeKeys.PGA,
				"cont_pgv" => TagTypeKeys.PGV,
				_ => throw new NotSupportedException(),
			};
		}
		public static double ExtractPropertyValue(USGSContour props) {
			ThrowHelper.ThrowIfNull(props);
			return ExtractPropertyValue(props.Value, props.Units);
		}
		public static double ExtractPropertyValue(float value, string unit) {
			return value * unit switch {
				"pctg" => 9.80665,
				"g" => 980.665,
				_ => 1,
			};
		}
		public static void ApplyCommonProductInfo(ReportModel result, USGSEarthquakeProduct product, string fileName, USGSProductContent content, IMessageStringSet res, CultureInfo culture) {
			ThrowHelper.ThrowIfNull(result);
			ThrowHelper.ThrowIfNull(product);
			ThrowHelper.ThrowIfNull(content);
			result.GroupKeys.Add(new USGSProductUnitKey(product.Source, product.Type, product.Code, fileName));
			result.RevisionKey = new USGSProductRevisionKey(content.LastModified);
			result.Source = string.Format(culture, res.GetStringRequired("AuthorityNameForwarded"), result.Source, product.Source);
		}
	}
}