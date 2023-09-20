using ProjNet.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Tiles
{
    public class SlippyMapTile
    {
        public SlippyMapTile(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        private readonly int x, y, z;

        public int X => x;
        public int Y => y;
        public int Z => z;

        public int Size => 1 << z;
        public string Id => $"({x},{y},{z})";

        public override string ToString() => Id;

        public bool IsValid()
        {
            if (z < 0 || z > 20)
            {
                return false;
            }

            if (x < 0 || x >= Size)
            {
                return false;
            }

            if (y < 0 || y >= Size)
            {
                return false;
            }

            return true;
        }

        public SlippyMapTile Parent()
        {
            if (Z == 0)
            {
                return null;
            }

            return new SlippyMapTile(X / 2, Y / 2, Z - 1);
        }

        public IEnumerable<SlippyMapTile> Children()
        {
            for (var x = 0; x < 2; x++)
            {
                for (var y = 0; y < 2; y++)
                {
                    yield return new SlippyMapTile(X * 2 + x, Y * 2 + y, Z + 1);
                }
            }
        }

        public LatLngBounds Bounds()
        {
            var min = SlippyMath.TileToLatLng(new (x, y), z);
            var max = SlippyMath.TileToLatLng(new (x + 1, y + 1), z);
            return new LatLngBounds(min, max);
        }

        public static SlippyMapTile ById(string id)
        {
            id = id.Replace("(", "").Replace(")", "");
            var parts = id.Split(',');
            if (parts.Length != 3)
            {
                throw new FormatException();
            }

            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);
            var z = int.Parse(parts[2]);
            return new SlippyMapTile(x, y, z);
        }


        public static SlippyMapTile ByCoords(LatLng coords, int zoomLevel)
        {
            var pos = SlippyMath.LatLngToTile(coords, zoomLevel);
            return new SlippyMapTile((int)pos.X, (int)pos.Y, zoomLevel);
        }


        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SlippyMapTile other)
            {
                return Id == other.Id;
            }

            return false;
        }
    }
}
