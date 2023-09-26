using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps;

namespace BlazorHeightmaps
{

	/// <summary>
	/// Describes details about the heightmap that will be generated. Contains stuff like desired bounds, data set, etc...
	/// </summary>
	public class HeightmapSpecification
	{

		/// <summary>
		/// Lat/lng bounds of the height map. If the aspect ratio of these bounds is not equal to the desired pixel size,
		/// the bounds should be shrunk so they match the desired pixel size.
		/// </summary>
		public LatLngBounds Bounds { get; set; }

		/// <summary>
		/// desired pixel size
		/// </summary>
		public Vector2 PixelSize { get; set; }

	}
}
