using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;


namespace Ambacht.Common.Maps.Tiles
{
	public interface IMapTile
	{
		string Key { get; }

		string Url { get; }

		string Crs { get; }

		Rectangle<double> Bounds { get; }

	}
}
