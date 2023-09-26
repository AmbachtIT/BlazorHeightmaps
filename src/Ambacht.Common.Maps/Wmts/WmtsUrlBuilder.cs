using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Wmts
{

	/// <summary>
	/// https://developers.arcgis.com/rest/services-reference/enterprise/wmts-tile-map-service-.htm
	/// </summary>
	public record class WmtsUrlBuilder
	{

		/// <summary>
		/// Including https, excluding trailing slash
		/// </summary>
		public string BaseUrl { get; set; }

		public string WmtsVersion { get; set; }

		/// <summary>
		/// Layer. Example: 0
		/// </summary>
		public string Layer { get; set; }

		public string Style { get; set; } = "default";

		/// <summary>
		/// Example: google_maps
		/// </summary>
		public string TileMatrixSet { get; set; }

		/// <summary>
		/// Example: tileMatrix0
		/// </summary>
		public string TileMatrix { get; set; }

		public int TileRow { get; set; }

		public int TileCol { get; set; }

		/// <summary>
		/// Example: png
		/// </summary>
		public string Format { get; set; }


		public string Build()
		{
			ArgumentException.ThrowIfNullOrEmpty(BaseUrl);
			ArgumentException.ThrowIfNullOrEmpty(WmtsVersion);
			ArgumentException.ThrowIfNullOrEmpty(Layer);
			ArgumentException.ThrowIfNullOrEmpty(Style);
			ArgumentException.ThrowIfNullOrEmpty(TileMatrixSet);
			ArgumentException.ThrowIfNullOrEmpty(TileMatrix);
			return
				$"{BaseUrl}/tile/{WmtsVersion}/{Layer}/{Style}/{TileMatrixSet}/{TileMatrix}/{TileRow}/{TileCol}.{Format}";
		}

	}


}
