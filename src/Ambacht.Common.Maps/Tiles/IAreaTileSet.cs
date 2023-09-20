using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Tiles
{
	public interface IAreaTileSet
	{

		AreaTile GetTile(Vector2 coords);

		IEnumerable<AreaTile> GetTiles(Rectangle bounds);

	}


}
