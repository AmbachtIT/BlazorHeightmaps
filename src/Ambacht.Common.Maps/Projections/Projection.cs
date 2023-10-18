using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;


namespace Ambacht.Common.Maps.Projections
{
    public abstract class Projection
    {

        public abstract Vector2<double> Project(LatLng pos);

        public abstract LatLng Invert(Vector2<double> pos);

    }


    public static class ProjectionExtensions
    {


    public static Vector3<double> Project(this Projection projection, LatLngAltitude pos) => 
            projection.Project(pos.ToLatLng()).ToVector3(pos.Altitude);

        public static LatLngAltitude Invert(this Projection projection, Vector3<double> v) =>
            projection.Invert(v.ToVector2()).WithAltitude(v.Z);


      public static LatLng Invert(this Projection projection, Vector2 v) =>
          projection.Invert(new(v.X, v.Y));

      public static LatLngAltitude Invert(this Projection projection, Vector3 v) =>
        projection.Invert(new(v.X, v.Y)).WithAltitude(v.Z);


    public static Rectangle<double> Project(this Projection projection, LatLngBounds bounds)
        {
	        var pos1 = projection.Project(bounds.SouthWest);
	        var pos2 = projection.Project(bounds.NorthEast);
	        if (pos2.X < pos1.X)
	        {
		        throw new Exception();
	        }
	        if (pos2.Y < pos1.Y)
	        {
		        throw new Exception();
	        }
	        return new Rectangle<double>(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y);
		}

        public static LatLngBounds Invert(this Projection projection, Rectangle<double> bounds)
        {
	        var pos1 = projection.Invert(bounds.TopLeft());
	        var pos2 = projection.Invert(bounds.BottomRight());
	        return new LatLngBounds(pos1, pos2);
        }


	}
}
