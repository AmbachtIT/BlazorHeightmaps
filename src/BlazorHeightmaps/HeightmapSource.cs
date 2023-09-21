using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps.Heightmaps;
using Ambacht.Common.Maps.Tiles;

namespace BlazorHeightmaps
{
	public class HeightmapSource
	{

		public IMapTileSet TileSet { get; set; }

		public IHeightmapReader HeightmapReader { get; set; }

	}
}
