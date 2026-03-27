using Cryville.EEW.Report;
using System;
using System.Collections.Generic;

namespace Cryville.EEW.JMA {
	public record JMAEventUnitKey(string Title, string EditorialOffice, string Status, string? EventID) : IReportUnitKey {
		static readonly Dictionary<string, HashSet<string>> _coverMap = new() {
			{ "震源・震度に関する情報", ["緊急地震速報（予報）", "緊急地震速報（警報）", "緊急地震速報（地震動予報）", "震度速報", "震源に関する情報"] },
			{ "津波警報・注意報・予報a", ["震源に関する情報"] },
			{ "降灰予報（速報）", ["降灰予報（詳細）"] },
			{ "降灰予報（詳細）", ["降灰予報（速報）"] },
		};
		static readonly HashSet<string> _switchableTitles = [
			"震度速報", "震源に関する情報", "顕著な地震の震源要素更新のお知らせ", "地震回数に関する情報", "地震の活動状況等に関する情報",
			"震源・震度に関する情報", "緊急地震速報（予報）", "緊急地震速報（警報）", "津波情報a", "津波警報・注意報・予報a", "地震・津波に関するお知らせ",
			"緊急地震速報配信テスト",
			"沖合の津波観測に関する情報",
			"南海トラフ地震臨時情報", "南海トラフ地震関連解説情報",
			"緊急地震速報（地震動予報）", "長周期地震動に関する観測情報",
			"北海道・三陸沖後発地震注意情報",
		];

		bool EqualsExceptTitle(JMAEventUnitKey other) =>
			(EditorialOffice == other.EditorialOffice || (_switchableTitles.Contains(Title) && _switchableTitles.Contains(other.Title)))
			&& Status == other.Status
			&& EventID == other.EventID;

		public virtual bool Equals(JMAEventUnitKey? other) => other != null && Title == other.Title && EqualsExceptTitle(other);
		public override int GetHashCode() => HashCode.Combine(Title, Status, EventID);

		public bool IsCoveredBy(IReportUnitKey key) {
			if (key is not JMAEventUnitKey other) return false;
			if (Equals(other)) return true;
			if (!_coverMap.TryGetValue(other.Title, out var list)) return false;
			return list.Contains(Title) && EqualsExceptTitle(other);
		}
	}
}
