using System.Collections.Generic;
using Ambacht.Common.Maps.Kml;

namespace Ambacht.Common.Maps
{
    public class Polygon
    {

        /// <summary>
        /// First and last point should be the same.
        /// </summary>
        public List<LatLng> Points { get; set; } = new List<LatLng>();

        public List<Polygon> Holes { get; set; } = new List<Polygon>();

        public bool IsClosed()
        {
            return Points.Count > 0 && Points.First() == Points.Last();
        }



        

    }
}