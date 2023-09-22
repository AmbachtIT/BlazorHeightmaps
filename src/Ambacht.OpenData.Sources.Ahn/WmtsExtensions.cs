using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps.Wmts;

namespace Ambacht.OpenData.Sources.Ahn
{

	/// <summary>
	/// https://geoforum.nl/t/ogc-services-wms-wmts-wcs-voor-ahn1-2-3/8531
	/// </summary>
	public static class WmtsExtensions
	{

		public static WmtsUrlBuilder Ahn4Ellipsis(this WmtsUrlBuilder builder)
		{
			return builder
			with
			{
				BaseUrl = $"https://api.stg1.ellipsis-drive.com/v3/ogc/wmts/73e91774-b6bb-4f57-ba49-2e0b1a68e71a?token=undefined&requestedEpsg=28992",
				Format = "png"
			};
		}

		public static WmtsUrlBuilder Ahn4Dsm5(this WmtsUrlBuilder builder, int row, int col)
		{
			return builder.Ahn4Ellipsis()
				with
				{
					TileMatrixSet = "matrix_14",
					TileMatrix = "14",
					WmtsVersion	= "1.0.0",
					TileRow = row,
					TileCol = col
				};

		}


	}
}
