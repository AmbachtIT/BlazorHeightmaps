using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Tiles
{
    public static class SlippyMath
    {

        private static float LongitudeToTileX(float lon, int z)
        {
            return (lon + 180.0f) / 360.0f * (1 << z);
        }

        private static float LatitudeToTileY(float lat, int z)
        {
            return (1 - MathF.Log(MathF.Tan(ToRadians(lat)) + 1 / MathF.Cos(ToRadians(lat))) / MathF.PI) / 2 * (1 << z);
        }

        private static float TileXToLongitude(float x, int z)
        {
            return x / (float)(1 << z) * 360.0f - 180f;
        }

        private static float TileYToLatitude(float y, int z)
        {
            float n = MathF.PI - 2.0f * MathF.PI * y / (1 << z);
            return 180.0f / MathF.PI * MathF.Atan(0.5f * (MathF.Exp(n) - MathF.Exp(-n)));
        }

        private static float ToRadians(float degrees)
        {
            return MathF.PI * degrees / 180.0f;
        }


        /// <summary>
        /// Converts the lat/lng coordinates to tile coordinates.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static Vector2 LatLngToTile(LatLng coords, int zoomLevel)
        {
            return new Vector2(
                x: LongitudeToTileX(coords.Longitude, zoomLevel),
                y: LatitudeToTileY(coords.Latitude, zoomLevel));
        }

        /// <summary>
        /// Converts 
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static LatLng TileToLatLng(Vector2 xy, int zoomLevel)
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
        public static Vector2 LatLngToPixel(LatLng coords, int zoomLevel, int tileSize)
        {
            return LatLngToTile(coords, zoomLevel) * tileSize;
        }

        /// <summary>
        /// Converts 
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public static LatLng PixelToLatLng(Vector2 xy, int zoomLevel, int tileSize)
        {
            return TileToLatLng(xy / tileSize, zoomLevel);
        }

    }
}
