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

        public void CopyTo(Heightmap target, int tx, int ty)
        {
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


        
    }
}
