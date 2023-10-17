using ProjNet.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;

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
        /// Rotation angle, in degrees
        /// </summary>
        public float Angle { get; set; }


        public SlippyMapView Pan(Vector2 delta)
        {
            var currentCoords = LatLngToView(this.Coords);
            currentCoords -= delta;
            var newCoords = ViewToLatLng(currentCoords);
            return this with
            {
              Coords = newCoords
            };
        }

        /// <summary>
        /// Converts the lat/lng to view coordinates (x, y) = ([0-width], [y-height])
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public Vector2 LatLngToView(LatLng coords)
        {
            var position = SlippyMath.LatLngToPixel(coords, ZoomLevel, TileSize);
            position -= SlippyMath.LatLngToPixel(this.Coords, ZoomLevel, TileSize);
            position = MathUtil.Rotate(position, MathUtil.DegreesToRadiansF(Angle));
            position += Size / 2;
            return position;
        }



    /// <summary>
    /// Converts the view coordinates (x, y) = ([0-width], [y-height]) to lat/lng 
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public LatLng ViewToLatLng(Vector2 position)
          {
            position -= Size / 2;
            position = MathUtil.Rotate(position, MathUtil.DegreesToRadiansF(-Angle));

            position += SlippyMath.LatLngToPixel(this.Coords, ZoomLevel, TileSize);
            return SlippyMath.PixelToLatLng(position, ZoomLevel, TileSize);
        }

    public IEnumerable<SlippyMapTile> GetVisibleTiles()
        {
            if (TileSize <= 0)
            {
                yield break;
            }
            var tileCount = 1 << ZoomLevel;
            var center = SlippyMath.LatLngToPixel(this.Coords, ZoomLevel, TileSize);
            var left = (int)Math.Floor(center.X - Size.X / 1.4);
            var top = (int)Math.Floor(center.Y - Size.Y / 1.4);
            var sx = left / TileSize;
            var sy = top / TileSize;

            for (var dy = 0; dy <= Math.Ceiling(Size.Y * 1.5 / TileSize); dy++)
            {
                for (var dx = 0; dx <= Math.Ceiling(Size.X * 1.5 / TileSize); dx++)
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
