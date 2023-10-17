using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Tiles
{
	public class SlippyTileViewModel
	{
		public string Key { get; set; }

    /// <summary>
    /// Coordinates of center of tile
    /// </summary>
		public LatLng Coords { get; set; }

		/// <summary>
		/// Url of the image
		/// </summary>
		public string Image { get; set; }

		public string Style { get; set; }

		public void UpdateView(SlippyMapView view)
		{
        // Position of center of tile, in component coordinates
			  var position = view.LatLngToView(Coords);
        position -= new Vector2(view.TileSize / 2f, view.TileSize / 2f);
        

        Style = $"transform: translate({(int)position.X}px, {(int)position.Y}px) rotate({(int)view.Angle}deg)";
    }

	}
}
