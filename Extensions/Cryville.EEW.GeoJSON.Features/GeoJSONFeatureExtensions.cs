using Cryville.Common.Compat;
using Cryville.EEW.Features;
using System;
using System.Linq;

namespace Cryville.EEW.GeoJSON.Features {
	public static class GeoJSONFeatureExtensions {
		/// <summary>
		/// Converts a position to an instance of the <see cref="Coordinates" /> struct.
		/// </summary>
		/// <returns>The converted coordinates.</returns>
		public static Coordinates ToCoordinates(this Position p) => p.ToCoordinates();
		/// <summary>
		/// Converts the current instance to an instance of the <see cref="EEW.Features.Geometry" /> class.
		/// </summary>
		/// <returns>The current instance converted to an instance of the <see cref="EEW.Features.Geometry" /> class.</returns>
		public static EEW.Features.Geometry ToFeaturesGeometry(this Geometry geo) {
			ThrowHelper.ThrowIfNull(geo);
			return geo switch {
				Point point => ToFeaturesGeometry(point),
				MultiPoint multiPoint => ToFeaturesGeometry(multiPoint),
				LineString lineString => ToFeaturesGeometry(lineString),
				MultiLineString multiLineString => ToFeaturesGeometry(multiLineString),
				Polygon polygon => ToFeaturesGeometry(polygon),
				MultiPolygon multiPolygon => ToFeaturesGeometry(multiPolygon),
				GeometryCollection geometryCollection => ToFeaturesGeometry(geometryCollection),
				_ => throw new NotSupportedException(),
			};
		}
		/// <inheritdoc cref="ToFeaturesGeometry(Geometry)" />
		public static EEW.Features.Point ToFeaturesGeometry(this Point geo) {
			ThrowHelper.ThrowIfNull(geo);
			return new EEW.Features.Point(geo.Coordinates.ToCoordinates());
		}
		/// <inheritdoc cref="ToFeaturesGeometry(Geometry)" />
		public static EEW.Features.MultiPoint ToFeaturesGeometry(this MultiPoint geo) {
			ThrowHelper.ThrowIfNull(geo);
			return new EEW.Features.MultiPoint([.. geo.Coordinates.Select(ToCoordinates)]);
		}
		/// <inheritdoc cref="ToFeaturesGeometry(Geometry)" />
		public static EEW.Features.LineString ToFeaturesGeometry(this LineString geo) {
			ThrowHelper.ThrowIfNull(geo);
			return new EEW.Features.LineString([.. geo.Coordinates.Select(ToCoordinates)]);
		}
		/// <inheritdoc cref="ToFeaturesGeometry(Geometry)" />
		public static EEW.Features.MultiLineString ToFeaturesGeometry(this MultiLineString geo) {
			ThrowHelper.ThrowIfNull(geo);
			return new EEW.Features.MultiLineString([.. geo.Coordinates.Select(c => c.Select(ToCoordinates).ToArray())]);
		}
		/// <inheritdoc cref="ToFeaturesGeometry(Geometry)" />
		public static EEW.Features.Polygon ToFeaturesGeometry(this Polygon geo) {
			ThrowHelper.ThrowIfNull(geo);
			return new EEW.Features.Polygon([.. geo.Coordinates.Select(c => c.Select(ToCoordinates).ToArray())]);
		}
		/// <inheritdoc cref="ToFeaturesGeometry(Geometry)" />
		public static EEW.Features.MultiPolygon ToFeaturesGeometry(this MultiPolygon geo) {
			ThrowHelper.ThrowIfNull(geo);
			return new EEW.Features.MultiPolygon([.. geo.Coordinates.Select(c => c.Select(c => c.Select(ToCoordinates).ToArray()).ToArray())]);
		}
		/// <inheritdoc cref="ToFeaturesGeometry(Geometry)" />
		public static EEW.Features.GeometryCollection ToFeaturesGeometry(this GeometryCollection geo) {
			ThrowHelper.ThrowIfNull(geo);
			return new EEW.Features.GeometryCollection([.. geo.Geometries.Select(ToFeaturesGeometry)]);
		}
	}
}
