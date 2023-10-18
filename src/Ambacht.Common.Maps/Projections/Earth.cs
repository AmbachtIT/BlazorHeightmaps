using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Projections
{
    public static class Earth
    {

        /// <summary>
        /// Earth radius in meters
        /// </summary>
        public const double Radius = 6_371_000;

        public const double Circumference = Radius * Math.PI * 2;

        public const double MetersPerDegree = Circumference / 360;


        public static double GetMetersPerDegree(double latitude)
        {
            return Math.Cos(latitude * Math.PI / 180.0) * MetersPerDegree;
        }
    }
}
