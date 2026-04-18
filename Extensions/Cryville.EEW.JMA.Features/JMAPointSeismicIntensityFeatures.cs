using Cryville.EEW.Features;
using Cryville.EEW.JMA.Map;
using System.Collections.Generic;
using System.Linq;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.JMA.Features {
	public class JMAPointSeismicIntensityFeatures {
		static JMAPointSeismicIntensityFeatures? s_instance;
		public static JMAPointSeismicIntensityFeatures Instance => s_instance ??= new();

		public IReadOnlyDictionary<string, Feature> Points { get; private set; }
		JMAPointSeismicIntensityFeatures() {
			Points = JMASeisStationList.Instance.Stations.ToDictionary(
				f => f.Code,
				f => new Feature(new Point(f.Longitude, f.Latitude)) {
					{ Is, ManMadeMonitoringStation },
					{ MonitoringSeismicActivity, true },
					{ Ref, f.Code },
					{ Name, (LocalizableCollection<string>)[
						new Localized<string>(f.Name, Local.Culture),
						new Localized<string>(f.Furigana, Local.CultureHrkt),
					] },
					{ Operator, (LocalizableCollection<string>)[
						new Localized<string>(f.Affiliation, Local.Culture),
					] },
				}
			);
		}
	}
}
