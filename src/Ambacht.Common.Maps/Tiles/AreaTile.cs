using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;
using Rectangle = Ambacht.Common.Mathmatics.Rectangle;

namespace Ambacht.Common.Maps.Tiles
{
	public class AreaTile
	{
		public string Key { get; set; }

		public string Url { get; set; }

		public string Crs { get; set; }

		public Rectangle Rectangle { get; set; }

	}
}
