namespace Cryville.EEW.JMAAtom.Features {
	public static class ExtraTagTypeKeys {
		public static readonly TagTypeKey Tide = "Tide";
		public static readonly TagTypeKey TideHigh = Tide.OfSubtype("High");
		public static readonly TagTypeKey TsunamiArrival = "TsunamiArrival";

		public static readonly TagTypeKey Polarity = "Polarity";
		public static readonly TagTypeKey SeismicWavePeriod = "SeismicWavePeriod";
		public static readonly TagTypeKey Sva = "Sva";
	}
}
