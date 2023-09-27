using System.Numerics;
using Ambacht.Common.IO;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Arcgis;
using Ambacht.Common.Maps.Heightmaps;
using Ambacht.Common.Maps.Projections;
using Ambacht.Common.Maps.Tiles;
using Ambacht.Common.Mathmatics;
using Ambacht.OpenData.Sources.Ahn;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHeightmaps.Test
{
	[TestFixture()]
	public class TestHeightmapGenerator
	{

		[Test(), Explicit("Downloads large files"), TestCaseSource(nameof(AllProviders))]
		public async Task BaseCase(IHeightmapDataProvider dataProvider)
		{
			var location = "utrecht";
			var game = "cities-skylines-1";
			var basePath = TestHelper.GetSourceRoot("test", "BlazorHeightmaps.Test", "data", location, game);
			var filename = $"{dataProvider}.png";

			if (!Directory.Exists(basePath))
			{
				Directory.CreateDirectory(basePath);
			}

			var spec = new HeightmapSpecification()
			{
				Bounds = GetUtrechtBounds(),
				PixelSize = new Vector2(1081, 1081)
			};

			var generator = new HeightmapGenerator();
			var source = new HeightmapSource()
			{
				DataProvider = dataProvider
			};

			var heightmap = await generator.Run(spec, source);
			Assert.That(heightmap.Width, Is.EqualTo(1081));
			Assert.That(heightmap.Height, Is.EqualTo(1081));

			Console.WriteLine($"Heightmap bounds: {heightmap.Bounds.ToString()}");

			var render = HeightmapRenders.Png16BitGreyscale with
			{
				FlipY = true,

			};
			using (var stream = File.Create(Path.Combine(basePath, filename.Replace(".png", ".hmz"))))
			{
				heightmap.Save(stream);
			}
			await heightmap.SaveImage(Path.Combine(basePath, filename.Replace(".png", "-full-range.png")), render);
			heightmap.Multiply(2);
			render = render with
			{
				MinValue = -40,
				MaxValue = 988,
			};
			await heightmap.SaveImage(Path.Combine(basePath, filename), render);
		}


		private LatLngBounds GetUtrechtBounds() => _projection.Invert(Rectangle.Around(new Vector2(135273, 453774), new(17280, 17280)));

		private readonly Projection _projection = new RijksDriehoeksProjection();

		[Test()]
		public async Task GetUrlFromArcGisAhnDataForUtrecht()
		{
			var rd = new Vector2(135273, 453774);
			var rdw = 17280;
			var rdh = 17280;
			var bounds = new Rectangle(rd.X - rdw / 2f, rd.Y - rdh / 2f, rdw, rdh);

			var url = new ArcgisExportImageUrlBuilder("https://ahn.arcgisonline.nl/arcgis/rest/services/AHNviewer/AHN4_DSM_50cm/ImageServer/exportImage")
			{
				Width = 1081,
				Height = 1081,
				Format = "png",
				//PixelType = "F32",
				BoundingBox = bounds,
				BoundingBoxSR = Crs.Rd,
				ImageSr = Crs.Rd
			}.Build();
			Console.WriteLine(url);
			await Task.CompletedTask;
		}



		private static IEnumerable<IHeightmapDataProvider> AllProviders()
		{
			var client = new HttpClient();
			var tiff = new TiledTiffHeightmapReader()
			{
				FlipY = true
			};
			yield return new StitchingHeightmapDataProvider(
				new AhnSheetMapTileSet(AhnRasterDataset.Ahn4_Dsm_0_5m),
				tiff,
				new CachingTileStreamSource(
					new HttpMapTileStreamSource(client),
					new LocalFileSystem(TestHelper.GetSourceRoot(".cache", "ahn4"))
				));
			yield return new ArcgisAhnHeightmapDataProvider(tiff, client);
		}



	}

}