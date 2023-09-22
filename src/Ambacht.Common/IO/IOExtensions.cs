using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;

namespace Ambacht.Common.IO
{
	public static class IOExtensions
	{

		public static async Task<Stream> WrapWithDecompression(this Stream stream, string filename)
		{
			if (UseGZip(filename))
			{
				return new GZipInputStream(stream);
			}
			if (filename.EndsWith("zip"))
			{
				var ms = new MemoryStream();
				await stream.CopyToAsync(ms);
				ms.Seek(0, SeekOrigin.Begin);

				var zip = new ZipFile(ms);
				return zip.GetInputStream(0);
			}
			return stream;
		}

		private static bool UseGZip(string filename)
		{
			var lower = filename.ToLowerInvariant();
			return lower.EndsWith(".gz") || lower.EndsWith("kmz");
		}



	}
}
