using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;


namespace Ambacht.Common.Maps.Arcgis
{

	/// <summary>
	/// https://developers.arcgis.com/rest/services-reference/enterprise/export-image.htm
	/// </summary>
	public record class ArcgisExportImageUrlBuilder
	{


		private readonly string _baseUrl;

		public ArcgisExportImageUrlBuilder(string baseUrl)
		{
			if (!baseUrl.EndsWith("exportImage"))
			{
				throw new ArgumentException("Unexpected base url");
			}
			_baseUrl = baseUrl;
		}

		/// <summary>
		/// The response format. The default response format is HTML. If the format is image, the image bytes are directly streamed to the client.
		/// 
		/// Values: html | json | image | kmz
		/// </summary>
		public string F { get; set; } = "image";

		/// <summary>
		/// jpgpng | png | png8 | png24 | jpg | bmp | gif | tiff | png32 | bip | bsq | lerc
		/// </summary>
		public string Format { get; set; }

		/// <summary>
		/// C128 | C64 | F32 | F64 | S16 | S32 | S8 | U1 | U16 | U2 | U32 | U4 | U8 | UNKNOWN
		/// </summary>
		public string PixelType { get; set; }

		public Rectangle<double> BoundingBox { get; set; }

		public string BoundingBoxSR { get; set; }

		public string ImageSr { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		/// <summary>
		/// The pixel value representing no information.
		/// </summary>
		public float? NoData { get; set; }


		public string Build()
		{
			var builder = new StringBuilder(_baseUrl);
			builder.Append($"?f={F}");
			builder.Append($"&format={Format}");
			if (!string.IsNullOrEmpty(PixelType))
			{
				builder.Append($"&pixelType={PixelType}");
			}
			builder.Append($"&bbox={WebUtility.UrlEncode(BoundingBox.ToString("L,T,R,B"))}");
			builder.Append($"&bboxSR={BoundingBoxSR}");
			builder.Append($"&imageSR={ImageSr}");
			builder.Append($"&size={Width}%2C{Height}");
			if (NoData.HasValue)
			{
				builder.Append($"&noData={NoData}");
			}

			return builder.ToString();
		}

	}
}
