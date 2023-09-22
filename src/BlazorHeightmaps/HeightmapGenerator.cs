﻿using System.Numerics;
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

		public async Task<Heightmap> Run(HeightmapSpecification specification, HeightmapSource source)
		{
			// Convert lat/lng to coordinates local to the data set
			var local = GetLocalCoordinates(specification.Center);
			var size = specification.PixelSize * specification.Scale * GetUnitsPerPixel();
			var bounds = new Rectangle(local.X - size.X / 2, local.Y - size.Y / 2, size.X, size.Y);

			var heightmaps = new List<Heightmap>();
			foreach (var tile in source.TileSet.GetTiles(bounds))
			{
				var heightmap = await FetchHeightmap(tile, source.StreamSource, source.HeightmapReader);

				heightmap = heightmap.Downsample((int) specification.Scale);

				heightmap.Crs = tile.Crs;
				heightmap.Bounds = tile.Bounds;
				heightmaps.Add(heightmap);
			}

			var stitched = Heightmap.Stitch(heightmaps);

			return stitched.GetPixelArea(bounds.TopLeft(), (int) specification.PixelSize.X,
				(int) specification.PixelSize.Y);
		}


		private async Task<Heightmap> FetchHeightmap(IMapTile tile, IMapTileStreamSource source, IHeightmapReader reader)
		{
			var url = tile.Url;
			await using var stream = await source.GetStream(tile);
			await using var wrapped = await stream.WrapWithDecompression(url.Split('/').Last());
			var filename = url.Split('/').Last();
			return await reader.Load(filename, wrapped);
		}






		private Vector2 GetLocalCoordinates(LatLng coords) => new RijksDriehoeksProjection().Project(coords);

		private float GetUnitsPerPixel() => 0.5f;



	}
}