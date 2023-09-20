using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;

namespace Ambacht.Common.Maps.Projections
{
    public abstract class Projection
    {

        public abstract Vector2 Project(LatLng pos);

        public abstract LatLng Invert(Vector2 pos);

    }


    public static class ProjectionExtensions
    {

        public static Vector3 Project(this Projection projection, LatLngAltitude pos) => 
            projection.Project(pos.ToLatLng()).ToVector3(pos.Altitude);

        public static LatLngAltitude Invert(this Projection projection, Vector3 v) =>
            projection.Invert(v.ToVector2()).WithAltitude(v.Z);

    }
}
