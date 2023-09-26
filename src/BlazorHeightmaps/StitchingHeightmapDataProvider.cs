using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.IO;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Heightmaps;
using Ambacht.Common.Maps.Projections;
using Ambacht.Common.Maps.Tiles;
using Ambacht.Common.Mathmatics;

namespace BlazorHeightmaps
{
	public class StitchingHeightmapDataProvider : IHeightmapDataProvider
	{

		public StitchingHeightmapDataProvider(IMapTileSet tileSet, IHeightmapReader reader, IMapTileStreamSource source)
		{
			_tileSet = tileSet;
			_reader = reader;
			_source = source;
		}

		private readonly IMapTileSet _tileSet;
		private readonly IHeightmapReader _reader;
		private readonly IMapTileStreamSource _source;


		public async Task<Heightmap> GetHeightmap(LatLng center, Vector2 pixelSize, float scale)
		{
			var local = GetLocalCoordinates(center);
			var size = pixelSize * scale * GetUnitsPerPixel();
			var bounds = new Rectangle(local.X - size.X / 2, local.Y - size.Y / 2, size.X, size.Y);

			var heightmaps = new List<Heightmap>();
			foreach (var tile in _tileSet.GetTiles(bounds))
			{
				var heightmap = await FetchHeightmap(tile);

				heightmap = heightmap.Downsample((int)scale);

				heightmap.Crs = tile.Crs;
				heightmap.Bounds = tile.Bounds;
				heightmaps.Add(heightmap);
			}

			var stitched = Heightmap.Stitch(heightmaps);

			return stitched.GetPixelArea(bounds.TopLeft(), (int)pixelSize.X,
				(int)pixelSize.Y);
		}

		private async Task<Heightmap> FetchHeightmap(IMapTile tile)
		{
			var url = tile.Url;
			await using var stream = await _source.GetStream(tile);
			await using var wrapped = await stream.WrapWithDecompression(url.Split('/').Last());
			var filename = url.Split('/').Last();
			return await _reader.Load(filename, wrapped);
		}

		private Vector2 GetLocalCoordinates(LatLng coords) => new RijksDriehoeksProjection().Project(coords);

		private float GetUnitsPerPixel() => 0.5f;
	}
}
