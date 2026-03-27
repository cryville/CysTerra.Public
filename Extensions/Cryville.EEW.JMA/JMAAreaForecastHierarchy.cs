using Cryville.EEW.JMA.Resources;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Cryville.EEW.JMA {
	public class JMAAreaForecastHierarchy {
		static JMAAreaForecastHierarchy? s_instance;
		public static JMAAreaForecastHierarchy Instance => s_instance ??= new();

		public IReadOnlyDictionary<string, string[]> HierarchyChildren { get; private set; }
		public IReadOnlyDictionary<string, string> HierarchyParent { get; private set; }
		public JMAAreaForecastHierarchy() {
			HierarchyChildren = JsonSerializer.Deserialize(JMAAreaForecastEEWHierarchy.Data, SerializerContext.Default.DictionaryStringStringArray)
				?? throw new InvalidOperationException("Invalid JMAAreaForecastEEW data.");
			var hierarchyParent = new Dictionary<string, string>();
			foreach (var p in HierarchyChildren) {
				foreach (var c in p.Value) {
					hierarchyParent.Add(c, p.Key);
				}
			}
			HierarchyParent = hierarchyParent;
		}
	}
}
