using Cryville.EEW.GeoJSON;
using Cryville.EEW.GeoNet.Model;
using System;
using System.Collections.Generic;

namespace Cryville.EEW.GeoNet {
	sealed class GeoNetQuakeParametersComparer : IEqualityComparer<IFeature<GeoNetQuake>> {
		static GeoNetQuakeParametersComparer? s_instance;
		public static GeoNetQuakeParametersComparer Instance => s_instance ??= new();
		public bool Equals(IFeature<GeoNetQuake>? x, IFeature<GeoNetQuake>? y) {
			if (x is null) return y is null;
			if (y is null) return false;
			if ((x.Geometry as Point)?.Coordinates != (y.Geometry as Point)?.Coordinates) return false;
			GeoNetQuake px = x.Properties, py = y.Properties;
			if (px.PublicID != py.PublicID) return false;
			if (px.Time != py.Time) return false;
			if (px.Depth != py.Depth) return false;
			if (px.Magnitude != py.Magnitude) return false;
			if (px.MMI != py.MMI) return false;
			if (px.Locality != py.Locality) return false;
			if (px.Quality != py.Quality) return false;
			return true;
		}
		public int GetHashCode(IFeature<GeoNetQuake> obj) {
			var props = obj.Properties;
			return HashCode.Combine((obj.Geometry as Point)?.Coordinates, props.PublicID, props.Time, props.Depth, props.Magnitude, props.MMI, props.Locality, props.Quality);
		}
	}
}
