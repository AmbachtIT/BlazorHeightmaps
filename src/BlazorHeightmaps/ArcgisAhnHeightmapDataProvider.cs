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
using Ambacht.OpenData.Sources.Ahn;

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
			var url = new ArcgisExportImageUrlBuilder($"https://ahn.arcgisonline.nl/arcgis/rest/services/AHNviewer/AHN4_{Layer.ToString().ToUpper()}_{FormattedResolution}/ImageServer/exportImage")
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

		

		public AhnLayer Layer { get; set; } = AhnLayer.Dsm;

		public AhnResolution Resolution { get; set; } = AhnResolution.Res_50cm;

		private string FormattedResolution => Resolution switch
		{
			AhnResolution.Res_5m => "5m",
			AhnResolution.Res_50cm => "50cm",
			_ => throw new NotSupportedException()
		};

		private readonly Projection _rd = new RijksDriehoeksProjection();

		public override string ToString()
		{
			return $"arcgisonline-{Layer.ToString().ToLower()}-{FormattedResolution}";
		}
	}
}
