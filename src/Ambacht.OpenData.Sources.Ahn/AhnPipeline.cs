using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.IO;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Heightmaps;
using Ambacht.Common.Maps.Tiles;
using ICSharpCode.SharpZipLib.Zip;
using Rectangle = Ambacht.Common.Mathmatics.Rectangle;

namespace Ambacht.OpenData.Sources.Ahn
{
    public class AhnPipeline
    {
        public AhnPipeline(IFileSystem files, IHttpClientFactory httpClientFactory)
        {
            _files = files;
            _httpClientFactory = httpClientFactory;
            this.SaveUnneededTiles = !(_files is InMemoryFileSystem);

#if DEBUG
            GenerateDebugImages = true;
#endif
        }

        private readonly IFileSystem _files;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly KaartbladenIndex _index = new KaartbladenIndex();

        public bool GenerateDebugImages { get; set; }

        public bool SaveUnneededTiles { get; set; }
        

        /// <summary>
        /// Stitches together multiple heightmaps
        /// </summary>
        /// <param name="set"></param>
        /// <param name="rd"></param>
        /// <returns></returns>
        public async Task<Heightmap> StitchTiles(AhnRasterDataset set, params Vector2[] rd)
        {
            // Collect heightmaps for all coordinates
            var tiles = rd.Select(c => _index.GetTile(set, c)).Distinct().ToList();
            var totalBounds = Rectangle.Cover(tiles.Select(h => h.Bounds));

            

            var result = new Heightmap(
                (int) (totalBounds.Width / set.MetersPerPixel),
                (int) (totalBounds.Height / set.MetersPerPixel))
            {
                Crs = Crs.RdEpsg,
                Bounds = totalBounds
            };

            var tileSizeMeters = KaartbladenIndex.TileSizePx * set.MetersPerPixel;
            for (var y = totalBounds.Top; y < totalBounds.Bottom; y += tileSizeMeters)
            {
                for (var x = totalBounds.Left; x < totalBounds.Right; x += tileSizeMeters)
                {
                    var newBounds = new Rectangle(x, y, tileSizeMeters, tileSizeMeters);
                    var heightmap = await GetTileHeightmap(set, newBounds.Center(), SaveUnneededTiles ? Rectangle.Empty : totalBounds);
                    var sx = (int)((heightmap.Bounds.Left - totalBounds.Left) / set.MetersPerPixel);
                    var sy = (int)((heightmap.Bounds.Top - totalBounds.Top) / set.MetersPerPixel);

                    heightmap.CopyTo(result, sx,sy);
                }
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public async Task<Heightmap> GetTileHeightmap(AhnRasterDataset set, Vector2 rd, Rectangle totalBounds)
        {
            var tile = _index.GetTile(set, rd);
            var result = await GetTileHeightmap(set, tile, totalBounds);
            return result;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="tileX">[0, 19]</param>
        /// <param name="tileY">[0, 24]</param>
        /// <returns></returns>
        public async Task<Heightmap> GetTileHeightmap(AhnRasterDataset set, AhnTile tile, Rectangle totalBounds)
        {
            var path = GetTilePath(set, tile);
            if (!await _files.Exists(path))
            {
                await SplitTiles(set, tile.Sheet, totalBounds);
            }

            await using var stream = await _files.OpenRead(path);
            return Heightmap.Load(stream);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This will load the entire 500MB image in memory, so concurrent runs are limited to two runs
        /// </remarks>
        /// <param name="version"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private async Task SplitTiles(AhnRasterDataset set, string sheet, Rectangle totalBounds)
        {
            try
            {
                var blad = _index.GetBlad(sheet);
                await _semaphore.WaitAsync();

                var source = await DownloadHeightmap(set, sheet);
                source.Crs = Crs.RdEpsg;
                source.Bounds = new Rectangle(blad.X, blad.Y, blad.Width, blad.Height);

                await new HeightmapTiler(_files)
                {
                    GenerateDebugImages = GenerateDebugImages,
                    Source = source,
                    GetFilenameFunc = (x, y)  => $"{sheet}-{x}-{y}.hmz",
                    Bounds = totalBounds,
                    PixelTiling = new Tiling(KaartbladenIndex.TileSizePx, KaartbladenIndex.TileSizePx),
                    OutputPath = $"{GetBasePath(set)}{sheet}/",
                    
                }.Run();

                if(GenerateDebugImages)
                {
                    // Write downsampled PNG for debug purposes
                    var downscaled = source.Downsample(4);
                    await using (var stream = await _files.OpenWrite($"{GetBasePath(set)}{sheet}/{sheet}.png"))
                    {
                        await downscaled.SaveImage(stream, HeightmapRenders.Png16BitGreyscale with
                        {
                            FlipY = true
                        });
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Heightmap> DownloadHeightmap(AhnRasterDataset set, string sheet)
        {
            using (var client = _httpClientFactory.CreateClient("Ahn"))
            {
                var url = set.GetDownloadLink(sheet);
                var stream = await GetStream(client, url);
                var filename = url.Split('/').Last();
                return await new TiledTiffHeightmapReader()
                {
                    FlipY = true
                }.Load(filename, stream);
            }
        }


        private async Task<Stream> GetStream(HttpClient client, string url)
        {
            var stream = await client.GetStreamAsync(url);
            if (url.EndsWith("zip"))
            {
                var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);

                var zip = new ZipFile(ms);
                return zip.GetInputStream(0);
            }
            

            return stream;
        }


        private string GetTilePath(AhnRasterDataset set, AhnTile tile) => $"{GetBasePath(set)}{tile.Sheet}/{tile.Sheet}-{tile.X}-{tile.Y}.hmz";

        private static SemaphoreSlim _semaphore = new SemaphoreSlim(2);

        private string GetBasePath(AhnRasterDataset set) => $"ahn{set.Version}/{set.Layer.ToString().ToLowerInvariant()}-{FormatResolution(set.Resolution)}/";

        private string FormatResolution(AhnResolution resolution) => resolution switch
        {
            AhnResolution.Res_50cm => "50cm",
            AhnResolution.Res_5m => "5m",
            _ => throw new NotImplementedException()
        };


    }


    public record class AhnTile
    {
        public string Sheet { get; init; }
        public int X { get; init; }
        public int Y { get; init; }

        public Rectangle Bounds { get; init; }
    }

}
