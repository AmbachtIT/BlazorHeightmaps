using System.Numerics;
using Ambacht.Common.Diagnostics;
using Ambacht.Common.IO;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Heightmaps;
using Ambacht.Common.Maps.Projections;
using Ambacht.Common.Maps.Tiles;
using Ambacht.Common.Mathmatics;
using ICSharpCode.SharpZipLib.Zip;

namespace BlazorHeightmaps
{
	public class HeightmapGenerator
	{

		public HeightmapGenerator(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		private readonly IHttpClientFactory _httpClientFactory;


		public async Task<Heightmap> Run(HeightmapSpecification specification, HeightmapSource source)
		{
			// Convert lat/lng to coordinates local to the data set
			var local = GetLocalCoordinates(specification.Center);
			var size = specification.PixelSize * GetUnitsPerPixel() / 2;
			var bounds = new Rectangle(local.X - size.X / 2, local.Y - size.Y / 2, size.X, size.Y);

			var heightmaps = new List<Heightmap>();
			foreach (var tile in source.TileSet.GetTiles(bounds))
			{
				var heightmap = await DownloadHeightmap(tile, source.HeightmapReader);
				heightmap.Crs = tile.Crs;
				heightmap.Bounds = tile.Bounds;
				heightmaps.Add(heightmap);
			}

			var stitched = Heightmap.Stitch(heightmaps);

			return stitched.GetPixelArea(bounds.TopLeft(), (int) specification.PixelSize.X,
				(int) specification.PixelSize.Y);
		}


		private async Task<Heightmap> DownloadHeightmap(IMapTile tile, IHeightmapReader reader)
		{
			using (var client = _httpClientFactory.CreateClient())
			{
				var url = tile.Url;
				var stream = await GetStream(client, url);
				var filename = url.Split('/').Last();
				return await reader.Load(filename, stream);
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



		private Vector2 GetLocalCoordinates(LatLng coords) => new RijksDriehoeksProjection().Project(coords);

		private float GetUnitsPerPixel() => 5;



	}
}