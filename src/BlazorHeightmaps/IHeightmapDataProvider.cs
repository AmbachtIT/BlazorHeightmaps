using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Heightmaps;
using Ambacht.Common.Mathmatics;

namespace BlazorHeightmaps
{
	public interface IHeightmapDataProvider
	{

		Task<Heightmap> GetHeightmap(LatLng center, Vector2 pixelSize, float scale);

	}
}
