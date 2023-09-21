using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;
using Rectangle = Ambacht.Common.Mathmatics.Rectangle;

namespace Ambacht.Common.Maps.Tiles
{
	public interface IMapTile
	{
		string Key { get; }

		string Url { get; }

		string Crs { get; }

		Rectangle Bounds { get; }

	}
}
