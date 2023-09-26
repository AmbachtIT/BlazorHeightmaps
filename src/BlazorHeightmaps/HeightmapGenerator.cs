using System.Numerics;
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
			return await source.DataProvider.GetHeightmap(specification.Bounds, specification.PixelSize);
		}


	}
}