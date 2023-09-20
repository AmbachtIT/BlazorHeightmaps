using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace Ambacht.Common.Maps.Kml
{

    public static class KmlHelper {


        public static string FormatColor(string htmlColor)
        {
            if (htmlColor.Length == 6)
            {
                return "ff" + htmlColor.Substring(4, 2) + htmlColor.Substring(2, 2) + htmlColor.Substring(0, 2);
            }

            if (htmlColor.StartsWith("#"))
            {
                return FormatColor(htmlColor.Substring(1));
            }

            if (htmlColor.Length == 8)
            {
                // This probably already is OK
                return htmlColor;
            }
            throw new ArgumentException($"Unexpected HTML color: {htmlColor}");
        }

		public static byte[] GetColor(byte a, byte r, byte g, byte b) {
			return new byte[] { a, b, g, r };
		}

		/*
		public static byte[] GetColor(byte r, byte g, byte b) {
			return new byte[] { 0xff, b, g, r };
		}

		public static byte[] GetColor(Color color) {
			return GetColor(color.AByte, color.RByte, color.GByte, color.BByte);
		}

		public static string FormatCoordinate(Projection projection, Vector2 location)
		{
			return FormatCoordinate(projection.ToLatLng(location));
		}
		*/


		public static string FormatCoordinate(LatLng latLng, double height = 0)
		{
			return string.Format(
				"{0},{1},{2}",
				latLng.Longitude.ToString(numberFormat),
				latLng.Latitude.ToString(numberFormat),
				height.ToString(numberFormat));
		}

        public static string FormatCoordinate(LatLngAltitude latLng)
        {
            return string.Format(
                "{0},{1},{2}",
                latLng.Longitude.ToString(numberFormat),
                latLng.Latitude.ToString(numberFormat),
                latLng.Altitude.ToString(numberFormat));
        }


        /*public static string FormatColor(Color color)
		{
			return string.Format("{0:x2}{1:x2}{2:x2}{3:x2}", color.AByte, color.BByte, color.GByte, color.RByte);
		}*/


        private static readonly IFormatProvider numberFormat = CultureInfo.InvariantCulture.NumberFormat;



	}
}
