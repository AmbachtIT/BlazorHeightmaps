using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;

namespace Ambacht.Common.Maps.Tiles
{
    public static class SlippyMath
    {

        private static double LongitudeToTileX(double lon, int z)
        {
            return (lon + 180.0f) / 360.0f * (1 << z);
        }

        private static double LatitudeToTileY(double lat, int z)
        {
            return (1 - Math.Log(Math.Tan(ToRadians(lat)) + 1 / Math.Cos(ToRadians(lat))) / Math.PI) / 2 * (1 << z);
        }

        private static double TileXToLongitude(double x, int z)
        {
            return x / (double)(1 << z) * 360.0f - 180f;
        }

        private static double TileYToLatitude(double y, int z)
        {
            double n = Math.PI - 2.0 * Math.PI * y / (1 << z);
            return 180.0 / Math.PI * Math.Atan(0.5 * (Math.Exp(n) - Math.Exp(-n)));
        }

        private static double ToRadians(double degrees)
        {
            return MathF.PI * degrees / 180.0;
        }


        /// <summary>
        /// Converts the lat/lng coordinates to tile coordinates.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static Vector2<double> LatLngToTile(LatLng coords, int zoomLevel)
        {
            return new Vector2<double>(
              LongitudeToTileX(coords.Longitude, zoomLevel),
              LatitudeToTileY(coords.Latitude, zoomLevel));
        }

        /// <summary>
        /// Converts 
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static LatLng TileToLatLng(Vector2<double> xy, int zoomLevel)
        {
            return new LatLng(TileYToLatitude(xy.Y, zoomLevel), TileXToLongitude(xy.X, zoomLevel));
        }

        /// <summary>
        /// Converts the lat/lng coordinates to tile coordinates.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static Vector2<double> LatLngToPixel(LatLng coords, int zoomLevel, int tileSize)
        {
            return LatLngToTile(coords, zoomLevel) * tileSize;
        }

        /// <summary>
        /// Converts 
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static LatLng PixelToLatLng(Vector2<double> xy, int zoomLevel, int tileSize)
        {
            return TileToLatLng(xy / tileSize, zoomLevel);
        }

    }
}
