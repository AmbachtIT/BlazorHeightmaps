using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ambacht.Common.Maps;
using Ambacht.Common.Mathmatics;
using BlazorHeightmaps.Presets;

namespace BlazorHeightmaps
{
	public class State
	{

		public LatLng Center { get; set; }

		public int ZoomLevel { get; set; }

		/// <summary>
		/// Bounds to draw
		/// </summary>
		public LatLngBounds Bounds =>
			new LatLngBounds(Center.Translate(SizeMeters / 2f), Center.Translate(-SizeMeters / 2f));

		/// <summary>
		/// Bounds to draw
		/// </summary>
		public LatLngBounds? BuildableBounds
		{
			get
			{
				if (BuildableSizeMeters == null)
				{
					return null;
				}

				var size = BuildableSizeMeters.Value;
				return new LatLngBounds(Center.Translate(size / 2f), Center.Translate(-size / 2f));
			}
		}


		public int MetersPerPixel { get; set; } = 16;
		public Vector2 SizePixels { get; set; } = new(1081, 1081);

		public Vector2 SizeMeters => SizePixels * MetersPerPixel;

		public Vector2? BuildableSizePixels { get; set; } = new Vector2(601, 601);

		public Vector2? BuildableSizeMeters => BuildableSizePixels * MetersPerPixel;


    public void OnStateChanged()
    {
      StateHasChanged.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler StateHasChanged;

    public void Apply(Preset preset)
    {
      SizePixels = preset.SizePixels;
      BuildableSizePixels = preset.BuildableSizePixels;
      MetersPerPixel = preset.MetersPerPixel;
      OnStateChanged();
    }
  }
}
