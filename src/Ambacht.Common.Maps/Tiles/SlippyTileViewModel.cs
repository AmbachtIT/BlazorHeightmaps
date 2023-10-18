using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;
using NetTopologySuite.Geometries;

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
        var halfSize = new Vector2<double>(-view.TileSize / 2.0);
        double size = view.TileSize;
        if (view.Angle % 90 == 0)
        {
          position = new Vector2<double>((int) position.X, (int) position.Y);
        }
        else
        {
          size *= 1.0 + (1.0 / view.TileSize);
        }


        Style = $"transform: translate({halfSize.X}px, {halfSize.Y}px) translate({position.X}px, {position.Y}px) rotate({view.Angle}deg);";

        if (size != view.TileSize)
        {
          Style += $"width: {size}px; height: {size}px;";
        }

    }

	}
}
