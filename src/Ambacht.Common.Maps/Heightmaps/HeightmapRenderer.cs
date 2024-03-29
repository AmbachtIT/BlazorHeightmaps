using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps.Heightmaps;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Ambacht.Common.Maps.Heightmaps
{

    public record class HeightmapRender<TPixel> where TPixel : unmanaged, IPixel<TPixel>
    {
        public TPixel NanValue { get; init; } = default;
        public float? MinValue { get; init; }
        public float? MaxValue { get; init; }
        public IImageEncoder Encoder { get; init; }

        public bool FlipY { get; init; }

        public Func<float, TPixel> GetPixelFunc { get; init; }

        public Image<TPixel> Render(Heightmap heightmap)
        {
            var result = new Image<TPixel>(heightmap.Width, heightmap.Height);
            var min = MinValue ?? heightmap.ValidHeights().Min();
            var max = MaxValue ?? heightmap.ValidHeights().Max();
            var delta = max - min;

            result.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < heightmap.Height; y++)
                {
                    var ry = y;
                    if (FlipY)
                    {
                        ry = heightmap.Height - ry - 1;
                    }
                    var row = accessor.GetRowSpan(ry);
                    for (var x = 0; x < heightmap.Width; x++)
                    {
                        var altitude = heightmap[x, y];
                        if (float.IsNaN(altitude))
                        {
                            row[x] = NanValue;
                        }
                        else
                        {
                            var alpha = Math.Clamp((altitude - min) / delta, 0, 1);
                            row[x] = GetPixelFunc(alpha);
                        }
                    }
                }
            });

            return result;
        }




    }

    public static class HeightmapRenders
    {

        public static HeightmapRender<L16> Png16BitGreyscale => new HeightmapRender<L16>()
        {
            Encoder = new PngEncoder()
            {
                ColorType = PngColorType.Grayscale,
                BitDepth = PngBitDepth.Bit16
            },
            GetPixelFunc = GetPixel16Greyscale()
        };

        public static Func<float, L16> GetPixel16Greyscale(ushort nanValue = default) => alpha =>
        {
	        if (float.IsNaN(alpha))
	        {
		        return new L16(nanValue);
	        }
            var value = alpha * 65535;
            return new L16(Convert.ToUInt16(value));
        };

        public static async Task SaveImage<TPixel>(this Heightmap heightmap, string path, HeightmapRender<TPixel> render)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            await using (var stream = File.Create(path))
            {
                await heightmap.SaveImage(stream, render);
            }
        }

        public static async Task SaveImage<TPixel>(this Heightmap heightmap, Stream stream, HeightmapRender<TPixel> render)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var image = render.Render(heightmap);
            await image.SaveAsync(stream, render.Encoder);
        }

    }
}
