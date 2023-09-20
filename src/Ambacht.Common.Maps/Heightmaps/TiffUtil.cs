using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMiracle.LibTiff.Classic;

namespace Ambacht.Common.Maps.Heightmaps
{
	public static class TiffUtil
	{

		public static Tiff OpenRead(string filename, Stream input)
		{
			var stream = input;
			if (!stream.CanSeek)
			{
				stream = new MemoryStream();
				input.CopyTo(stream);
				stream.Seek(0, SeekOrigin.Begin);
			}
			return Tiff.ClientOpen(filename, "r", stream, new TiffStream());
		}

	}
}
