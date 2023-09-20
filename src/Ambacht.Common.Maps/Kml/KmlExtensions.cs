using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using Ambacht.Common;

namespace Ambacht.Common.Maps.Kml
{

    public static class KmlExtensions {

		/*
		public static IEnumerable<IEnumerable<Coords>> GetCoords(this KmlType kml) {
			return kml.GetPlacemarks().SelectMany(p => GetCoordinates(p));
		}
		*/

		/*public static IEnumerable<PlacemarkType> GetPlacemarks(this KmlType kml) {
			if(kml.Item is PlacemarkType) {
				yield return kml.Item as PlacemarkType;
			} else if(kml.Item is DocumentType) {
				var doc = kml.Item as DocumentType;
				foreach(var placemark in doc.Features.OfType<PlacemarkType>()) {
					yield return placemark;
				}
			}
		}

		private static IEnumerable<IEnumerable<Coords>> GetCoordinates(PlacemarkType placemark) {
		if(placemark.Geometries != null) {
				foreach(var polygon in GetCoordinates(placemark.Geometries.OfType<PolygonType>())) {
					yield return polygon;
				}
			} 
		}

		private static IEnumerable<IEnumerable<Coords>> GetCoordinates(IEnumerable<PolygonType> polygons) {
			foreach(var polygon in polygons) {
				yield return GetCoordinates(polygon);
			}
		}

	
		public static IEnumerable<Coords> GetCoordinates(this PolygonType polygon) {
			return GetCoordinates(polygon.outerBoundaryIs.LinearRing.coordinates);
		}*/



		private static readonly IFormatProvider NumberFormat = CultureInfo.InvariantCulture.NumberFormat;

	}
}
