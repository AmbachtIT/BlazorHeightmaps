using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps.Heightmaps;
using BitMiracle.LibTiff.Classic;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Tiff;

namespace Ambacht.Common.Maps.Heightmaps
{
    public class TiledTiffHeightmapReader : IHeightmapReader
    {
        public bool FlipY { get; set; }



        public Task<Heightmap> Load(string filename, Stream stream, CancellationToken token = default)
        {
			using Tiff image = TiffUtil.OpenRead(filename, stream);


			// Find the width and height of the image
			var width = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
			var height = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

			var result = new Heightmap(width, height);

			var tileWidth = image.GetField(TiffTag.TILEWIDTH)[0].ToInt();
			var tileHeight = image.GetField(TiffTag.TILELENGTH)[0].ToInt();

			var buffer = new byte[image.TileSize()];

			for (var y = 0; y < height; y += tileHeight)
			{
				var currentTileHeight = Math.Min(height - y, tileHeight);
				for (var x = 0; x < width; x += tileWidth)
				{
					var currentTileWidth = Math.Min(width - x, tileWidth);

					if (image.ReadTile(buffer, 0, x, y, 0, 0) <= 0)
					{
						throw new InvalidOperationException();
					}

					for (var ty = 0; ty < currentTileHeight; ty++)
					{
						for (var tx = 0; tx < currentTileWidth; tx++)
						{
							var index = (ty * tileHeight + tx) * 4;
							var altitude = BitConverter.ToSingle(buffer, index);
							if (altitude < -1000 || altitude > 10_000)
							{
								altitude = float.NaN;
							}

							var ry = y + ty;
							if (FlipY)
							{
								ry = result.Height - ry - 1;
							}

							result[x + tx, ry] = altitude;
						}
					}
				}
			}

			return Task.FromResult(result);
		}

        public async Task<Heightmap> Load(string path, CancellationToken token = default)
        {
	        await using (var stream = File.OpenRead(path))
	        {
				var filename = new FileInfo(path).Name;
				return await Load(filename, stream, token);
	        }
        }

        public override string ToString() => "tiled-tiff";
    }
}
