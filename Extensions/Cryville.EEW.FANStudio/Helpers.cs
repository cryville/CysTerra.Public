using System.Collections.Generic;
using System.Linq;

namespace Cryville.EEW.FANStudio {
	public static class Helpers {
		public static IEnumerable<int> EnumerateRomanNumerals(string? str) => str?
			.Where(c => c is (>= '\x2160' and <= '\x216b') or (>= '\x2170' and <= '\x217b'))
			.Select(c => (c & 0xf) + 1)
			?? [];
	}
}
