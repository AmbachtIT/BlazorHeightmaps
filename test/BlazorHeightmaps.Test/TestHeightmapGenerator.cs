using System.Numerics;
using Ambacht.Common.IO;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Heightmaps;
using Ambacht.Common.Maps.Tiles;
using Ambacht.OpenData.Sources.Ahn;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHeightmaps.Test
{
	[TestFixture()]
	public class TestHeightmapGenerator
	{

		[Test(), Explicit("Downloads large files")]
		public async Task BaseCase()
		{
			var spec = new HeightmapSpecification()
			{
				Center = new LatLng(52.0833f, 5.0739f),
				PixelSize = new Vector2(1081, 1081)
			};
			await Test(spec, "cities-skylines-utrecht.png");
		}


		[Test(), Explicit("Downloads large files")]
		public async Task Scale25()
		{
			var spec = new HeightmapSpecification()
			{
				Center = new LatLng(52.0833f, 5.0739f),
				PixelSize = new Vector2(1081, 1081),
				Scale = 25
			};
			await Test(spec, "cities-skylines-utrecht-25.png");
		}



		private async Task Test(HeightmapSpecification spec, string filename)
		{
			using (var provider = TestHelper.CreateServiceProvider())
			{
				var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
				using var httpClient = httpClientFactory.CreateClient();
				var generator = new HeightmapGenerator();

				var source = new HeightmapSource()
				{
					DataProvider = CreateAhnStitchingProvider(httpClient)
				};

				var heightmap = await generator.Run(spec, source);
				Assert.That(heightmap.Width, Is.EqualTo(1081));
				Assert.That(heightmap.Height, Is.EqualTo(1081));

				var render = HeightmapRenders.Png16BitGreyscale with
				{
					FlipY = true
				};
				using (var stream = File.Create(TestHelper.GetSourceRoot("test", "BlazorHeightmaps.Test", "data",
					       filename.Replace(".png", ".hmz"))))
				{
					heightmap.Save(stream);
				}
				await heightmap.SaveImage(TestHelper.GetSourceRoot("test", "BlazorHeightmaps.Test", "data", filename), render);
			}
		}


		private IHeightmapDataProvider CreateAhnStitchingProvider(HttpClient client)
		{
			return new StitchingHeightmapDataProvider(
				new AhnSheetMapTileSet(AhnRasterDataset.Ahn4_Dsm_0_5m),
				new TiledTiffHeightmapReader()
				{
					FlipY = true
				},
				new CachingTileStreamSource(
					new HttpMapTileStreamSource(client),
					new LocalFileSystem(TestHelper.GetSourceRoot(".cache", "ahn4"))
				));
		}

	}

}