using System.Numerics;
using Ambacht.Common.Diagnostics;
using Ambacht.Common.IO;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Heightmaps;
using Ambacht.Common.Maps.Projections;
using Ambacht.Common.Mathmatics;
using Ambacht.OpenData.Sources.Ahn;

namespace BlazorHeightmaps
{
	public class HeightmapGenerator
	{
		public HeightmapGenerator(IFileSystem files, IHttpClientFactory httpClientFactory)
		{
			_files = files;
			_httpClientFactory = httpClientFactory;
		}

		private readonly IFileSystem _files;
		private readonly IHttpClientFactory _httpClientFactory;

		/// <summary>
		/// Used to report progress
		/// </summary>
		public Progress? Progress { get; set; }


		public async Task<Heightmap> Run(HeightmapSpecification specification)
		{
			var ahn = new AhnPipeline(_files, _httpClientFactory);

			// Convert lat/lng to coordinates local to the data set
			var local = GetLocalCoordinates(specification.Center);
			var size = specification.PixelSize * GetUnitsPerPixel() / 2;
			var bounds = new Rectangle(local.X - size.X / 2, local.Y - size.Y / 2, size.X, size.Y);

			var dataset = AhnRasterDataset.Ahn4_Dsm_0_5m;

			var stitched = await ahn.StitchTiles(dataset, bounds.Corners().ToArray());

			var result = new Heightmap((int) specification.PixelSize.X, (int) specification.PixelSize.Y);
			var alpha = MathUtil.ReverseLerp(stitched.Bounds.TopLeft(), stitched.Bounds.BottomRight(), bounds.TopLeft());
			var sx = (int)(alpha.X * stitched.Width);
			var sy = (int)(alpha.Y * stitched.Height);
			result.CopyFrom(stitched, sx, sy);
			return result;
		}



		private Vector2 GetLocalCoordinates(LatLng coords) => new RijksDriehoeksProjection().Project(coords);

		private float GetUnitsPerPixel() => 5;



	}
}