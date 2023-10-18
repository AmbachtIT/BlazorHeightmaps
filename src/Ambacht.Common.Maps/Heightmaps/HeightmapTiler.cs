using Ambacht.Common.IO;
using Ambacht.Common.Maps.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;

namespace Ambacht.Common.Maps.Heightmaps
{
    public class HeightmapTiler
    {

        public HeightmapTiler(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        private readonly IFileSystem _fileSystem;


        public Heightmap Source { get; set; }

        public Func<int, int, string> GetFilenameFunc { get; set; }

        public string OutputPath { get; set; }


        public Tiling PixelTiling { get; set; }
        public bool GenerateDebugImages { get; set; }

        /// <summary>
        /// If this is set, only tiles that overlap with the specified bounds are returned
        /// </summary>
        public Rectangle<double> Bounds { get; set; } = Rectangle<double>.Empty;


        public async Task Run()
        {
            PathHelper.ValidateDirectory(OutputPath);


            var tileWidthPx = (int)PixelTiling.TileWidth;
            var tileHeightPx = (int)PixelTiling.TileHeight;

            if (Source.Width % tileWidthPx != 0)
            {
                throw new ArgumentOutOfRangeException($"Image width {Source.Width} is not a multiple of {tileWidthPx}");
            }
            if (Source.Height % tileHeightPx != 0)
            {
                throw new ArgumentOutOfRangeException($"Image height {Source.Height} is not a multiple of {tileHeightPx}");
            }

            var tilesX = Source.Width / tileWidthPx;
            var tilesY = Source.Height / tileHeightPx;

            var tileWidthM = Source.Bounds.Width / tilesX;
            var tileHeightM = Source.Bounds.Height / tilesY;

            for (var tileY = 0; tileY < tilesY; tileY++)
            {
                for (var tileX = 0; tileX < tilesX; tileX++)
                {
	                var newBounds = new Rectangle<double>(
	                  Source.Bounds.Left + tileX * tileWidthM,
	                  Source.Bounds.Top + tileY * tileHeightM,
	                  tileWidthM,
	                  tileHeightM);

	                if (Bounds.HasArea && !Bounds.Overlaps(newBounds))
	                {
                        continue;
	                }

                    var targetHeightmap = new Heightmap(tileWidthPx, tileHeightPx)
                    {
                        Crs = Source.Crs,
                        Bounds = newBounds,
                    };

                    var sourceX = tileX * tileWidthPx;
                    var sourceY = tileY * tileHeightPx;

                    var filename = GetFilenameFunc(tileX, tileY);
                    targetHeightmap.CopyFrom(Source, sourceX, sourceY);

                    await using var stream = await _fileSystem.OpenWrite(OutputPath + filename);
                    targetHeightmap.Save(stream);
                }
            }

        }

    }
}
