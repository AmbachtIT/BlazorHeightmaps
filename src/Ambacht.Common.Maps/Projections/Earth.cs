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

        /// <summary>
        /// Earth radius in meters
        /// </summary>
        public const float RadiusF = 6_371_000;

        public const float CircumferenceF = RadiusF * MathF.PI * 2f;

        public const float MetersPerDegreeF = CircumferenceF / 360f;

        public static float GetMetersPerDegreeF(float latitude)
        {
            return MathF.Cos(latitude * MathF.PI / 180f) * MetersPerDegreeF;
        }
    }
}
