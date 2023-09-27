using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Arcgis;
using Ambacht.Common.Maps.Heightmaps;
using Ambacht.Common.Maps.Projections;
using Ambacht.Common.Mathmatics;

namespace BlazorHeightmaps
{
	public class ArcgisAhnHeightmapDataProvider : IHeightmapDataProvider
	{
		public ArcgisAhnHeightmapDataProvider(IHeightmapReader reader, HttpClient httpClient)
		{
			_reader = reader;
			_httpClient = httpClient;
		}


		private readonly HttpClient _httpClient;
		private readonly IHeightmapReader _reader;


		public async Task<Heightmap> GetHeightmap(LatLngBounds bounds, Vector2 pixelSize)
		{
			var rdBounds = _rd.Project(bounds).ExpandToMatchRatio(pixelSize);
			var url = new ArcgisExportImageUrlBuilder("https://ahn.arcgisonline.nl/arcgis/rest/services/AHNviewer/AHN4_DSM_50cm/ImageServer/exportImage")
			{
				Width = (int)pixelSize.X,
				Height = (int)pixelSize.Y,
				Format = "tiff",
				PixelType = "F32",
				NoData = -10,
				BoundingBox = rdBounds,
				BoundingBoxSR = Crs.Rd,
				ImageSr = Crs.Rd
			}.Build();

			await using (var stream = await _httpClient.GetStreamAsync(url))
			{
				var result = await _reader.Load("file.tiff", stream);
				result.Crs = Crs.RdEpsg;
				result.Bounds = rdBounds;
				return result;
			}
		}


		private readonly Projection _rd = new RijksDriehoeksProjection();

	}
}
