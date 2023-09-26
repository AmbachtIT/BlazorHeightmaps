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


		public async Task<Heightmap> GetHeightmap(LatLngBounds latLngBounds, Vector2 pixelSize)
		{
			var bounds = _projection.Project(latLngBounds).ExpandToMatchRatio(pixelSize);

			var desiredMetersPerPixel = bounds.Width / pixelSize.X;

			var heightmaps = new List<Heightmap>();
			var tiles = _tileSet.GetTiles(bounds).ToList();
			foreach (var tile in tiles)
			{
				var heightmap = await FetchHeightmap(tile);

				var actualMetersPerPixel = heightmap.UnitsPerPixel.X;
				var scale = (int)Math.Round(desiredMetersPerPixel / actualMetersPerPixel);

				heightmap = heightmap.Downsample(scale);

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
			var result = await _reader.Load(filename, wrapped);
			result.Bounds = tile.Bounds;
			return result;
		}

		private Vector2 GetLocalCoordinates(LatLng coords) => new RijksDriehoeksProjection().Project(coords);

		private float GetUnitsPerPixel() => 0.5f;


		public override string ToString() => $"stitching({_tileSet}, {_reader}, {_source})";

	}
}
