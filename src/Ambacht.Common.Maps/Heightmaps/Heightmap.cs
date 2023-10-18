using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Ambacht.Common.Mathmatics;
using Ambacht.Common.UX;
using ICSharpCode.SharpZipLib.GZip;


namespace Ambacht.Common.Maps.Heightmaps
{
    public class Heightmap : IEnumerable<float>
    {
        public Heightmap(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this._data = new float[width * height];
        }

        public int Width { get; }
        public int Height { get; }

        public string Crs { get; set; }

        public Rectangle<double> Bounds { get; set; }

        /// <summary>
        /// If strict mode is set to true, out of bounds indices will result in an OutOfRangeException, otherwise they will be silently ignored
        /// </summary>
        public bool StrictMode { get; set; } = true;

        public float DefaultValue { get; set; } = float.NaN;

        private readonly float[] _data;


        public float this[int x, int y]
        {
	        get
	        {
		        if (!Contains(x, y))
		        {
			        if (StrictMode)
			        {
				        throw new ArgumentOutOfRangeException($"Invalid index: {x}, {y}");
			        }
			        return float.NaN;
		        }
				return _data[x + y * Width]; 
	        }
	        set
	        {
		        if (!Contains(x, y))
		        {
			        if (StrictMode)
			        {
				        throw new ArgumentOutOfRangeException($"Invalid index: {x}, {y}");
			        }

			        return;
		        }
		        _data[x + y * Width] = value;
	        }
        }

        public IEnumerator<float> GetEnumerator() => ((IEnumerable<float>)_data).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();

		public void Clear(float? value = null)
		{
			var actual = value ?? DefaultValue;
			_data.AsSpan().Fill(actual);
		}





        public bool IsAcceptableDownscaleFactor(int factor)
        {
	        return Width % factor == 0 && Height % factor == 0;

        }

		public Heightmap Downsample(int factor)
        {
            var result = new Heightmap(Width / factor, Height / factor);
            for (var ny = 0; ny < result.Height; ny++)
            {
                for (var nx = 0; nx < result.Width; nx++)
                {
                    var sum = 0f;
                    var count = 0;
                    for (var sy = 0; sy < factor; sy++)
                    {
                        for (var sx = 0; sx < factor; sx++)
                        {
	                        var px = nx * factor + sx;
	                        var py = ny * factor + sy;
	                        if (px < Width && py < Height)
	                        {
		                        var value = this[px, py];
		                        if (!float.IsNaN(value))
		                        {
			                        sum += value;
			                        count++;
		                        }
							}
						}
                    }

                    if (count > 0)
                    {
	                    result[nx, ny] = sum / count;
                    }
                    else
                    {
                        result[nx, ny] = float.NaN;
                    }
				}
            }

            return result;
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

			for (var i = 0; i < _data.Length; i++)
			{
				writer.Write(_data[i]);
			}
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
            result.Bounds = new Rectangle<double>(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            for (var i = 0; i < result._data.Length; i++)
            {
                result._data[i] = reader.ReadSingle();
            }
            return result;
        }


        public const uint Magic = 0x7a6b5c4e;
        public const uint Version = 0x01;


        public bool Contains(int x, int y)
        {
	        return x >= 0 && x < Width
	            && y >= 0 && y < Height;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Automatically detects whether rescaling is necessary
		/// </remarks>
		/// <param name="target"></param>
		/// <exception cref="InvalidOperationException"></exception>
        public void CopyTo(Heightmap target)
        {
	        if (Crs != target.Crs)
	        {
		        throw new InvalidOperationException("This only works if source and source CRS are the same");
	        }

	        if ((UnitsPerPixel - target.UnitsPerPixel).Length() > .001)
	        {
		        throw new InvalidOperationException("Can't blit heightmap, they have different scales");
	        }

	        var alpha = MathUtil.ReverseLerp(target.Bounds.TopLeft(), target.Bounds.BottomRight(), Bounds.TopLeft());
	        var sx = (int)(alpha.X * target.Width);
			var sy = (int)(alpha.Y * target.Height);
            CopyTo(target, sx, sy);
        }

        public void CopyFromRescaling(Heightmap source)
        {
	        if (Crs != source.Crs)
	        {
		        throw new InvalidOperationException("This only works if source and source CRS are the same");
	        }

	        for (var y = 0; y < Height; y++)
	        {
		        for (var x = 0; x < Width; x++)
		        {
			        var pixelBounds = GetPixelBounds(x, y);
			        if (!pixelBounds.Overlaps(source.Bounds))
			        {
						continue;
			        }

			        this[x, y] = source.GetInterpolatedValue(pixelBounds);
		        }
	        }
        }

        public float GetInterpolatedValue(Rectangle<double> rect)
        {
	        var count = 0f;
			var total = 0f;
	        var (fromX, fromY) = GetIndex(rect.TopLeft());
	        var (toX, toY) = GetIndex(rect.BottomRight());
	        for (var y = fromY; y <= toY; y++)
	        {
		        if (y < 0 || y >= Height)
		        {
					continue;
		        }

		        for (var x = fromX; x <= toX; x++)
		        {
			        if (x < 0 || x >= Width)
			        {
				        continue;
			        }

			        var value = this[x, y];
			        if (!float.IsNaN(value))
			        {
				        count++;
						total += value;
			        }
		        }
	        }

	        if (count == 0f)
	        {
				return float.NaN;
	        }
			return total / count;
		}


		/// <summary>
		/// Gets index of specified coordinates
		/// </summary>
		public (int, int) GetIndex(Vector2<double> pos)
		{
			var lerped = MathUtil.ReverseLerp(Bounds.TopLeft(), Bounds.BottomRight(), pos);
			return ((int) (lerped.X * Width), (int) (lerped.Y * Height));
		}

        public Rectangle<double> GetPixelBounds(int x, int y)
        {
	        var pixelWidth = Bounds.Width / Width;
			var pixelHeight = Bounds.Height / Height;
			return new Rectangle<double>(
				Bounds.Left + x * pixelWidth,
				Bounds.Top + y * pixelHeight,
				pixelWidth,
				pixelHeight
			);
        }

		public Span<float> GetRowSpan(int y)
        {
	        if (y < 0 || y >= Height)
	        {
		        if (StrictMode)
		        {
					throw new ArgumentOutOfRangeException(nameof(y));
		        }
				return Span<float>.Empty;
	        }

	        return _data.AsSpan(y * Width, Width);
        }

        public Span<float> GetRowSpan(int y, Range<int> xRange)
        {
	        if (y < 0 || y >= Height)
	        {
		        if (StrictMode)
		        {
			        throw new ArgumentOutOfRangeException(nameof(y));
		        }
		        return Span<float>.Empty;
	        }

	        return _data.AsSpan(y * Width, Width);
        }

		public void CopyTo(Heightmap target, int tx, int ty)
        {
            // TODO: Use Span<T> to speed this up
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
	                if (target.Contains(tx + x, ty + y))
	                {
		                target[tx + x, ty + y] = this[x, y];
	                }
				}
            }
        }

        public void CopyFrom(Heightmap source, int sx, int sy)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
	                if (source.Contains(sx + x, sy + y))
	                {
		                this[x, y] = source[sx + x, sy + y];
	                }
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

	        var bounds = RectangleUtil.Cover(list.Select(l => l.Bounds));
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
        public Heightmap GetPixelArea(Vector2<double> pos, int width, int height)
        {
	        if (!Bounds.HasArea)
	        {
		        throw new InvalidOperationException();
	        }

	        var result = new Heightmap(width, height)
	        {
                Crs = Crs,
                Bounds = new Rectangle<double>(pos.X, pos.Y, width * Bounds.Width / Width, height * Bounds.Height / Height)
			};
	        var alpha = MathUtil.ReverseLerp<double>(Bounds.TopLeft(), Bounds.BottomRight(), pos);
	        var sx = (int)(alpha.X * Width);
	        var sy = (int)(alpha.Y * Height);
	        result.CopyFrom(this, sx, sy);
	        return result;
        }


        public Vector2<double> UnitsPerPixel => new(Bounds.Width / Width, Bounds.Height / Height);


		/// <summary>
		/// Multiply all data with a fixed value
		/// </summary>
		/// <param name="value"></param>
        public void Multiply(float value)
        {
	        for (var i = 0; i < _data.Length; i++)
	        {
				_data[i] *= value;
	        }
        }
    }
}
