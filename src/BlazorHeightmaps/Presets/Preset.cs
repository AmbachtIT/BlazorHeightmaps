using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BlazorHeightmaps.Presets
{
	public class Preset
	{

		public string Name { get; init; }

    public override string ToString() => Name;

    public int MetersPerPixel { get; init; }
		public Vector2 SizePixels { get; init; }

		public Vector2? BuildableSizePixels { get; init; }


		public static readonly Preset CitiesSkylines = new Preset()
		{
			Name = "Cities: Skylines 1",
			SizePixels = new(1081, 1081),
			BuildableSizePixels = new(601, 601),
			MetersPerPixel = 16
		};



		public static IEnumerable<Preset> All()
		{
			yield return CitiesSkylines;

			foreach (var (name, px) in new[]
			         {
				         ("Tiny", 1024),
				         ("Small", 2048),
				         ("Medium", 2816),
				         ("Large", 3584),
				         ("Very Large", 4096),
				         ("Huge*", 5120),
				         ("Megalomaniac*", 6144),
			         })
			{
				yield return new Preset()
				{
					Name = $"Transport Fever 2 / {name} 1x1",
					MetersPerPixel = 4,
					SizePixels = new(px + 1, px + 1)
				};
			}
		}

    public static Preset? ByName(string name) => All().SingleOrDefault(p => p.Name == name);
  }
}
