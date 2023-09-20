﻿using System;
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
		/// Center of the height map
		/// </summary>
		public LatLng Center { get; set; }

		/// <summary>
		/// desired pixel size
		/// </summary>
		public Vector2 PixelSize { get; set; }


	}
}
