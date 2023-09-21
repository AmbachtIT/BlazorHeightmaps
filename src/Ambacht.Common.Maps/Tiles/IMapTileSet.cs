using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;
using Rectangle = Ambacht.Common.Mathmatics.Rectangle;

namespace Ambacht.Common.Maps.Tiles
{
	public interface IMapTileSet
	{

		IMapTile GetTile(Vector2 coords);

		IEnumerable<IMapTile> GetTiles(Rectangle bounds);

	}


	public static class IMapTileSetExtensions
	{
		public static IEnumerable<IMapTile> GetTiles(this IMapTileSet set, IEnumerable<Vector2> points)
		{
			var bounds = Rectangle.Cover(points.Select(p => set.GetTile(p).Bounds));
			return set.GetTiles(bounds);
		}
	}


}
