using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Ambacht.Common.Mathmatics;
using Ambacht.Common.UX;
using ICSharpCode.SharpZipLib.GZip;
using Rectangle = Ambacht.Common.Mathmatics.Rectangle;

namespace Ambacht.Common.Maps.Heightmaps
{
    public class Heightmap : IEnumerable<float>
    {
        public Heightmap(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this._data = new float[width, height];
        }

        public int Width { get; }
        public int Height { get; }

        public string Crs { get; set; }

        public Rectangle Bounds { get; set; }

        private readonly float[,] _data;


        public float this[int x, int y]
        {
            get => _data[x, y];
            set
            {
                if (float.IsInfinity(value))
                {
                    throw new ArgumentException("float.infinity is not supported for height maps");
                }
                _data[x, y] = value;
            }
        }

        public IEnumerator<float> GetEnumerator()
        {
            foreach (var f in _data)
            {
                yield return f;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void Save(Stream stream)
        {
            using var writer = new BinaryWriter(new GZipOutputStream(stream));
            writer.Write(Magic);
            writer.Write(Version);
            writer.Write(Width);
            writer.Write(Height);
            writer.Write(Crs ?? "");
            writer.Write(Bounds.Left);
            writer.Write(Bounds.Top);
            writer.Write(Bounds.Width);
            writer.Write(Bounds.Height);

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    writer.Write(this[x, y]);
                }
            }
        }


        public Heightmap Downsample(int factor)
        {
            if (Width % factor != 0 || Height % factor != 0)
            {
                throw new ArgumentException("Downsample factor should be a multiple of both height and width");
            }

            var result = new Heightmap(Width / factor, Height / factor);
            for (var ny = 0; ny < result.Height; ny++)
            {
                for (var nx = 0; nx < result.Width; nx++)
                {
                    var sum = 0f;
                    for (var sy = 0; sy < factor; sy++)
                    {
                        for (var sx = 0; sx < factor; sx++)
                        {
                            sum += this[nx * factor + sx, ny * factor + sy];
                        }
                    }

                    result[nx, ny] = sum / (factor * factor);
                }
            }

            return result;
        }


        public static Heightmap Load(Stream stream)
        {
            using var reader = new BinaryReader(new GZipInputStream(stream), Encoding.UTF8);
            if (reader.ReadUInt32() != Magic || reader.ReadUInt32() != Version)
            {
                throw new InvalidOperationException($"Unexpected file contents");
            }

            var result = new Heightmap(reader.ReadInt32(), reader.ReadInt32())
            {
                Crs = reader.ReadString(),
            };
            result.Bounds = new Rectangle(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            for (var y = 0; y < result.Height; y++)
            {
                for (var x = 0; x < result.Width; x++)
                {
                    result[x, y] = reader.ReadSingle();
                }
            }

            return result;
        }


        public const uint Magic = 0x7a6b5c4e;
        public const uint Version = 0x01;


        public void CopyTo(Heightmap target)
        {
	        if (Crs != target.Crs)
	        {
		        throw new InvalidOperationException("This only works if source and target CRS are the same");
	        }
	        var alpha = MathUtil.ReverseLerp(target.Bounds.TopLeft(), target.Bounds.BottomRight(), Bounds.TopLeft());
	        var sx = (int)(alpha.X * target.Width);
			var sy = (int)(alpha.Y * target.Height);
            CopyTo(target, sx, sy);
        }

		public void CopyTo(Heightmap target, int tx, int ty)
        {
            // TODO: Use Span<T> to speed this up
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    target[tx + x, ty + y] = this[x, y];
                }
            }
        }

        public void CopyFrom(Heightmap source, int sx, int sy)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    this[x, y] = source[sx + x, sy + y];
                }
            }
        }

        public IEnumerable<float> ValidHeights() => this.Where(v => !float.IsNaN(v));

        /// <summary>
        /// Creates a new heightmap that contains all the provided heightmaps. They need to have the same CRS and specified bounds for this to work
        /// </summary>
        /// <param name="heightmaps"></param>
        /// <returns></returns>
        public static Heightmap Stitch(IEnumerable<Heightmap> heightmaps)
        {
	        var list = heightmaps.ToList();

	        var bounds = Rectangle.Cover(list.Select(l => l.Bounds));
	        if (!bounds.HasArea)
	        {
		        throw new InvalidOperationException("Heightmaps need to have non-empty bounds for this to work");
	        }

	        var crs = list.Select(h => h.Crs).Distinct().Single();
	        var scaleX = list.Select(h => h.Bounds.Width / h.Width).Distinct().Single();
	        var scaleY = list.Select(h => h.Bounds.Height / h.Height).Distinct().Single();

	        if (scaleY <= 0 || scaleX <= 0)
	        {
		        throw new InvalidOperationException("Invalid scale");
	        }

	        var result = new Heightmap((int) (bounds.Width / scaleX), (int) (bounds.Height / scaleY))
	        {
                Bounds = bounds,
                Crs = crs
	        };

	        foreach (var heightmap in list)
	        {
                heightmap.CopyTo(result);
	        }

	        return result;
        }

        /// <summary>
        /// Cuts a pixel-perfect slice of this heightmap, starting at the specified position. The heightmap must have bounds for this to work
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Heightmap GetPixelArea(Vector2 pos, int width, int height)
        {
	        if (!Bounds.HasArea)
	        {
		        throw new InvalidOperationException();
	        }

	        var result = new Heightmap(width, height)
	        {
                Crs = Crs,
                Bounds = new Rectangle(pos.X, pos.Y, width * Bounds.Width / Width, height * Bounds.Height / Height)
			};
	        var alpha = MathUtil.ReverseLerp(Bounds.TopLeft(), Bounds.BottomRight(), pos);
	        var sx = (int)(alpha.X * Width);
	        var sy = (int)(alpha.Y * Height);
	        result.CopyFrom(this, sx, sy);
	        return result;
        }
    }
}
