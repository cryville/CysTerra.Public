using Cryville.EEW.Features;
using Cryville.EEW.JMA.Map;
using System.Collections.Generic;
using System.Linq;
using static Cryville.EEW.TagTypeKeys;

namespace Cryville.EEW.JMA.Features {
	public class JMAPointTsunamiFeatures {
		static JMAPointTsunamiFeatures? s_instance;
		public static JMAPointTsunamiFeatures Instance => s_instance ??= new();

		public IReadOnlyDictionary<string, Feature> Points { get; private set; }
		JMAPointTsunamiFeatures() {
			Points = JMAPointTsunamiList.Instance.Points.ToDictionary(
				f => f.Key,
				f => new Feature(new Point(new(f.Value.Longitude / 60.0, f.Value.Latitude / 60.0))) {
					{ Is, ManMadeMonitoringStation },
					{ MonitoringTideGauge, true },
					{ Ref, f.Key },
					{ Name, (LocalizableCollection<string>)[
						new Localized<string>(f.Value.Name, Local.Culture),
						JMAMessages.PointTsunami().RootMessageStringSet.GetStringRequired(f.Key),
					] },
					{ Operator, (LocalizableCollection<string>)[
						new Localized<string>(f.Value.Affiliation, Local.Culture),
					] },
				}
			);
		}
	}
}
