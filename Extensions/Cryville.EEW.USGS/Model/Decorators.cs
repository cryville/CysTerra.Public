namespace Cryville.EEW.USGS.Model {
	public record USGSContoursProduct(
		USGSContours Self,
		USGSEarthquakeProduct Product,
		string FileName,
		USGSProductContent Content
	) : USGSContours(Self.Features, Self.BoundingBox, Self.Name, Self.Metadata);
}
