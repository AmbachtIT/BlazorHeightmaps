using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Tiles
{
	public class SlippyTileViewModel
	{
		public string Key { get; set; }
		public LatLng Coords { get; set; }

		/// <summary>
		/// Position, in component coordinates
		/// </summary>
		public Point Position { get; set; }

		/// <summary>
		/// Url of the image
		/// </summary>
		public string Image { get; set; }

		public string Style => $"left: {Position.X}px; top: {Position.Y}px;";

		public void UpdateView(SlippyMapView view)
		{
			this.Position = view.LatLngToView(Coords);
		}

	}
}
