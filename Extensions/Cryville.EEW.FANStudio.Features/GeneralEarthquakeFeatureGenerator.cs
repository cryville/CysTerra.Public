using Cryville.Common.Compat;
using Cryville.EEW.FANStudio.Model;
using Cryville.EEW.Features;
using Cryville.Measure;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.FANStudio.Features {
	public abstract class GeneralEarthquakeFeatureGenerator<T> : IGenerator<T, Feature> where T : GeneralEarthquake {
		protected virtual CultureInfo LocalCulture => Local.UnitedStatesCulture;
		protected virtual string ProcessLocalLocationName(string name) => name;
		protected virtual object GetMagnitudeQuantity(float magnitude) => new QuantityInc(magnitude, 0.05f, Units.Dimensionless);
		protected virtual object GetDepthQuantity(float depth) => new QuantityInc(depth, 0.5f, DerivedMeasures.Kilometre);

		public Feature Generate(T e, ref CultureInfo culture) {
			ThrowHelper.ThrowIfNull(e);

			var f = new Feature {
				{ Is, Earthquake },
				{ Time, new DateTimeOffset(e.ShockTime, Local.TimeZoneOffset) },
				{ At, GenerateFromHypocenter(e) },
			};
			if (e.Magnitude is float mag)
				f.Add(Magnitude, GetMagnitudeQuantity(mag));
			return f;
		}
		Feature GenerateFromHypocenter(T e) {
			var f = new Feature(new Point(e.Longitude, e.Latitude)) {
				{ Is, Hypocenter },
				{ Name, new Localized<string>(ProcessLocalLocationName(e.PlaceName), LocalCulture) },
			};
			if (e.Depth is float depth)
				f.Add(HypocenterDepth, GetDepthQuantity(depth));
			return f;
		}
	}

	public class EMSCEarthquakeFeatureGenerator : GeneralEarthquakeFeatureGenerator<EMSCEarthquake> {
		protected override CultureInfo LocalCulture => Local.GreatBritainCulture;
	}
	public partial class BCSFEarthquakeFeatureGenerator : GeneralEarthquakeFeatureGenerator<BCSFEarthquake> {
#if NET7_0_OR_GREATER
		[GeneratedRegex(@"^.*? of magnitude [0-9\.]+,\s*")]
		private static partial Regex MagnitudePrefixRegex();
#else
		static readonly Regex r_MagnitudePrefixRegex = new(@"^.*? of magnitude [0-9\.]+,\s*");
		static Regex MagnitudePrefixRegex() => r_MagnitudePrefixRegex;
#endif
		protected override string ProcessLocalLocationName(string name) => MagnitudePrefixRegex().Replace(name, "");
		protected override object GetMagnitudeQuantity(float magnitude) => new Quantity(magnitude, Units.Dimensionless);
	}
	public class GFZEarthquakeFeatureGenerator : GeneralEarthquakeFeatureGenerator<GFZEarthquake> {
		protected override object GetMagnitudeQuantity(float magnitude) => new QuantityInc(magnitude, 0.005f, Units.Dimensionless);
	}
	public partial class USPEarthquakeFeatureGenerator : GeneralEarthquakeFeatureGenerator<USPEarthquake> {
		protected override object GetMagnitudeQuantity(float magnitude) => new Quantity(magnitude, Units.Dimensionless);
	}
}
