using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;


namespace Ambacht.Common.Maps.Tiles
{

    /// <summary>
    /// A way of subdividing a 2D space in a rectangular grid
    /// </summary>
    public class Tiling
    {

        public Tiling(float tileWidth, float tileHeight)
        {
            if (tileWidth <= 0 || tileHeight <= 0)
            {
                throw new ArgumentException("Tile size can't be zero");
            }
            this.TileWidth = tileWidth; 
            this.TileHeight = tileHeight;
        }

        public float TileWidth { get; }

        public float TileHeight { get; }

        public Tile GetTile(Vector2 pos)
        {
            var coords = GetTileCoordinates(pos);
            return new Tile()
            {
                X = coords.Item1,
                Y = coords.Item2,
                Bounds = new Rectangle<double>(coords.Item1 * TileWidth, coords.Item2 * TileHeight, TileWidth, TileHeight)
            };
        }

        public (int, int) GetTileCoordinates(Vector2 pos) => ((int)(pos.X / TileWidth), (int)(pos.Y / TileHeight));


    }

    public class Tile
    {
        public int X { get; init; }
        public int Y { get; init; }

        public Rectangle<double> Bounds { get; init; }

        public Vector2<double> GetCoordinates(Vector2<double> world) => world - new Vector2<double>(Bounds.Left, Bounds.Top);
    }
}
