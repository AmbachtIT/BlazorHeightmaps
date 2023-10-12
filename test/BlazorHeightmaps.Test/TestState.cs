using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps;

namespace BlazorHeightmaps.Test
{

	[TestFixture()]
	public class TestState
	{

		[Test()]
		public void TestTranslateBoundsX()
		{
			var previous = new LatLng();
			var previousDelta = new LatLng();

			for (var i = 0; i < 10; i++)
			{
				var current = previous.Translate(new Vector2(10_000, 10_000));
				var currentDelta = current - previous;

				if (i > 0)
				{

					Assert.That(currentDelta.Latitude, Is.Not.EqualTo(0));
					Assert.That(Math.Abs(currentDelta.Latitude - previousDelta.Latitude), Is.LessThan(0.0001));

					Assert.That(currentDelta.Longitude, Is.Not.EqualTo(0));
					Assert.That(Math.Abs(currentDelta.Longitude - previousDelta.Longitude), Is.GreaterThan(0.0001));
				}

				previous = current;
				previousDelta = currentDelta;
			}
		}



	}
}
