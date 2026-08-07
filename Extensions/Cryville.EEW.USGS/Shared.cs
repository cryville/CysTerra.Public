using QuakeML;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Cryville.EEW.USGS {
	static class Shared {
#if NET5_0_OR_GREATER
		[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Quakeml))]
		[UnconditionalSuppressMessage("Trimming", "IL2026")]
#endif
		public static readonly XmlSerializer QuakeMLSerializer = new(typeof(Quakeml));
	}
}
