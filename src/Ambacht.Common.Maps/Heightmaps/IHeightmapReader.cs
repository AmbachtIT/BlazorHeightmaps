using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Heightmaps
{
	public interface IHeightmapReader
	{

		Task<Heightmap> Load(string filename, Stream stream, CancellationToken token = default);

	}
}
