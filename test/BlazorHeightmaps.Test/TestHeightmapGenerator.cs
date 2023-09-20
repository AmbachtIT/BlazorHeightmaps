using System.Numerics;
using Ambacht.Common.IO;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Heightmaps;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorHeightmaps.Test
{
	[TestFixture()]
	public class TestHeightmapGenerator
	{

		[Test(), Explicit("Downloads large files")]
		public async Task TestGenerator()
		{
			using (var provider = TestHelper.CreateServiceProvider())
			{
				var system = new InMemoryFileSystem();
				var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
				var generator = new HeightmapGenerator(system, httpClientFactory);
				var spec = new HeightmapSpecification()
				{
					Center = new LatLng(52.0833f, 5.0739f),
					PixelSize = new Vector2(1081, 1081)
				};
				var heightmap = await generator.Run(spec);
				Assert.That(heightmap.Width, Is.EqualTo(1081));
				Assert.That(heightmap.Height, Is.EqualTo(1081));

				var render = HeightmapRenders.Png16BitGreyscale;
				await heightmap.SaveImage(TestHelper.GetSourceRoot("test", "BlazorHeightmaps.Test", "data", "cities-skylines-utrecht.png"), render);
			}
		}
	}

}