using System;
using System.Diagnostics.CodeAnalysis;

namespace Cryville.EEW.Wolfx {
	public static class WolfxHelpers {
		[return: NotNullIfNotNull(nameof(id))]
		public static string? ExtractEventID(string? id) {
			if (id == null) return null;
			int index = id.IndexOf('_', StringComparison.Ordinal);
			if (index == -1) return id;
			return id[..index];
		}
	}
}
