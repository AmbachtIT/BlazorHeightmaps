using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps.Projections;
using Ambacht.Common.Mathmatics;

namespace Ambacht.Common.Maps.Tiles
{
	public class SlippyMapProjection : Projection
	{

			private readonly int _zoomLevel;

			public SlippyMapProjection(int zoomLevel)
			{
				_zoomLevel = zoomLevel;
			}

			public override Vector2<double> Project(LatLng pos) => SlippyMath.LatLngToTile(pos, _zoomLevel);

			public override LatLng Invert(Vector2<double> pos) => SlippyMath.TileToLatLng(pos, _zoomLevel);
	}
}
