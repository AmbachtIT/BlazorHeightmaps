using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps.Projections;

namespace Ambacht.Common.Maps
{
    public record struct LatLng(float Latitude, float Longitude)
    {
        public LatLngAltitude WithAltitude(float altitude) => new (Latitude, Longitude, altitude);


        /// <summary>
        /// Translates this position. NOTE: ONLY WORKS ON SMALL SCALES
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public LatLng Translate(Vector3 v) => Translate(new Vector2(v.X, v.Y));

        /// <summary>
        /// Translates this position. NOTE: ONLY WORKS ON SMALL SCALES
        /// </summary>
        /// <param name="v">translation in meters</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public LatLng Translate(Vector2 v)
        {
	        return new(
		        Latitude + v.Y / Earth.MetersPerDegreeF,
		        Longitude + v.X / Earth.GetMetersPerDegreeF(Latitude)
	        );
        }


        public static LatLng operator +(LatLng l1, LatLng l2) => new(l1.Latitude + l2.Latitude, l1.Longitude + l2.Longitude);
        public static LatLng operator -(LatLng l1, LatLng l2) => new(l1.Latitude - l2.Latitude, l1.Longitude - l2.Longitude);
	}

	public record struct LatLngAltitude(float Latitude, float Longitude, float Altitude)
    {
        public LatLng ToLatLng() => new LatLng(Latitude, Longitude);

        /// <summary>
        /// Translates this position. NOTE: ONLY WORKS ON SMALL SCALES
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public LatLngAltitude Translate(Vector3 v)
        {
            return new(
                Latitude + v.Y / Earth.MetersPerDegreeF,
                Longitude + v.X / Earth.GetMetersPerDegreeF(Latitude),
                Altitude + v.Z
            );
        }
    }

    public class LatLngBounds
    {
        public LatLngBounds(LatLng c1, LatLng c2)
        {
            this.min = new LatLng(Math.Min(c1.Latitude, c2.Latitude), Math.Min(c1.Longitude, c2.Longitude));
            this.max = new LatLng(Math.Max(c1.Latitude, c2.Latitude), Math.Max(c1.Longitude, c2.Longitude));
        }

        public readonly LatLng min, max;

        public LatLng Min => min;

        public LatLng Max => max;

        public LatLng NorthWest => new LatLng(Max.Latitude, Min.Longitude);
		public LatLng NorthEast => new LatLng(Max.Latitude, Max.Longitude);
		public LatLng SouthEast => new LatLng(Min.Latitude, Max.Longitude);
		public LatLng SouthWest => new LatLng(Min.Latitude, Min.Longitude);

        public LatLng Center() => new LatLng((Min.Latitude + Max.Latitude) / 2, (Min.Longitude + Max.Longitude) / 2);

    }
}
