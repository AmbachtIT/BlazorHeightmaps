using ProjNet.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Tiles
{
    public struct SlippyMapView
    {
        /// <summary>
        /// map coordinates the view is centered on
        /// </summary>
        public LatLng Coords { get; set; }

        /// <summary>
        /// Size of the viewport in pixels
        /// </summary>
        public Vector2 Size { get; set; }

        public Vector2 HalfSize => new(Size.X / 2, Size.Y / 2);

        /// <summary>
        /// Zoomlevel
        /// </summary>
        /// <remarks>
        /// The minimum zoom level is 0.
        ///
        /// The maximum zoom level depends on the tile set and usually is somewhere between 15 and 20. 
        /// </remarks>
        public float Zoom { get; set; }

        public int ZoomLevel => (int) Math.Ceiling(Zoom);

        /// <summary>
        /// Tile size, in pixels
        /// </summary>
        public int TileSize { get; set; }


        /// <summary>
        /// Converts the lat/lng coordinates to tile coordinates.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="coords"></param>
        /// <returns></returns>
        public Vector2 LatLngToTile(LatLng coords) => SlippyMath.LatLngToTile(coords, ZoomLevel);

        /// <summary>
        /// Converts 
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public LatLng TileToLatLng(Vector2 xy) => SlippyMath.TileToLatLng(xy, ZoomLevel);

        /// <summary>
        /// Converts the lat/lng coordinates to tile coordinates.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="coords"></param>
        /// <returns></returns>
        public Vector2 LatLngToPixel(LatLng coords) => SlippyMath.LatLngToPixel(coords, ZoomLevel, TileSize);


        /// <summary>
        /// Converts the lat/lng to view coordinates (x, y) = ([0-width], [y-height])
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public Point LatLngToView(LatLng coords)
        {
            var position = LatLngToPixel(coords);
            position -= LatLngToPixel(this.Coords);
            position += Size / 2;
            return new Point((int)position.X, (int)position.Y);
        }

        /// <summary>
        /// Converts 
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public LatLng PixelToLatLng(Vector2 xy) => SlippyMath.PixelToLatLng(xy, ZoomLevel, TileSize);


        public LatLngBounds GetBoundingRect()
        {
            var halfSize = Size / 2;
            var center = LatLngToPixel(Coords);
            return new LatLngBounds(
                PixelToLatLng(center - halfSize),
                PixelToLatLng(center + halfSize));
        }

        public IEnumerable<SlippyMapTile> GetVisibleTiles()
        {
            if (TileSize <= 0)
            {
                yield break;
            }
            var tileCount = 1 << ZoomLevel;
            var center = LatLngToPixel(Coords);
            var left = (int)Math.Floor(center.X - Size.X / 2.0);
            var top = (int)Math.Floor(center.Y - Size.Y / 2.0);
            var sx = left / TileSize;
            var sy = top / TileSize;

            for (var dy = 0; dy <= Math.Ceiling((float)Size.Y / TileSize); dy++)
            {
                for (var dx = 0; dx <= Math.Ceiling((float)Size.X / TileSize); dx++)
                {
                    var x = sx + dx;
                    var y = sy + dy;
                    if (x < tileCount && y < tileCount)
                    {
                        yield return new SlippyMapTile(x, y, ZoomLevel);
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{Coords.Latitude}, {Coords.Longitude} z{ZoomLevel} ({Size.X}, {Size.Y})";
        }
    }
}
