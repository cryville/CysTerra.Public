using Cryville.Common.Compat;
using Cryville.EEW.JMAAtom.Model;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Cryville.EEW.JMAAtom {
	public static class JMAAtomHelpers {
		const string OVERSEA_VOLCANIC_ERUPTION_COMMENT = "本情報の冒頭に「海外で規模の大きな地震がありました。」や「震源地」とありますが、これは「遠地地震に関する情報」を作成する際に自動的に付与される文言です。実際には、規模の大きな地震は発生していない点に留意してください。";

		public static bool IsOverseaVolcanicEruption(JMAReport e, [NotNullWhen(true)] out Model.Seismology.Body? seisBody) {
			ThrowHelper.ThrowIfNull(e);
			seisBody = null;
			if (e.Head.Title != "遠地地震に関する情報") return false;
			if (e.Body is not Model.Seismology.Body body) return false;
			seisBody = body;
			return body.Comments?.FreeFormComment?.Contains(OVERSEA_VOLCANIC_ERUPTION_COMMENT, StringComparison.Ordinal) ?? false;
		}
	}
}