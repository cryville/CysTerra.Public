using Cryville.EEW.ComponentModel;
using Cryville.EEW.Report;
using QuakeML;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cryville.EEW.QuakeML {
	[Export(typeof(IComponentCollector))]
	public class QuakeMLExtensionCollector : ComponentCollector<IGenerator<(Event, ReportModel), ReportModel>> {
		internal static QuakeMLExtensionCollector? Instance { get; private set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static IComponentCollector ManualInstance => Instance ?? new();
		QuakeMLExtensionCollector() { Instance = this; }

		public override string? GetName([NotNull] ref CultureInfo? culture) {
			using var lres = new LocalizedResource("", ref culture);
			var res = lres.RootMessageStringSet;
			return res.GetStringRequired("ExtensionCollector");
		}

		public override bool IsAutomatic => true;
	}
}
