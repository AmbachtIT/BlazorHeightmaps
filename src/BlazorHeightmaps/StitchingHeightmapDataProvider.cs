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
		private readonly Projection _projection = new RijksDriehoeksProjection();


		public async Task<Heightmap> GetHeightmap(LatLngBounds latLngBounds, Vector2<double> pixelSize)
		{
			var bounds = _projection.Project(latLngBounds).ExpandToMatchRatio(pixelSize);
			var result = new Heightmap((int) pixelSize.X, (int) pixelSize.Y)
			{
				Bounds = bounds
			};

			var tiles = _tileSet.GetTiles(bounds).ToList();
			foreach (var tile in tiles)
			{
				var heightmap = await FetchHeightmap(tile);

				result.CopyFromRescaling(heightmap);
			}

			return result;
		}

		private async Task<Heightmap> FetchHeightmap(IMapTile tile)
		{
			var url = tile.Url;
			await using var stream = await _source.GetStream(tile);
			await using var wrapped = await stream.WrapWithDecompression(url.Split('/').Last());
			var filename = url.Split('/').Last();
			var result = await _reader.Load(filename, wrapped);
			result.Bounds = tile.Bounds;
			return result;
		}

		private Vector2<double> GetLocalCoordinates(LatLng coords) => new RijksDriehoeksProjection().Project(coords);

		private float GetUnitsPerPixel() => 0.5f;


		public override string ToString() => $"stitching({_tileSet}, {_reader}, {_source})";

	}
}
