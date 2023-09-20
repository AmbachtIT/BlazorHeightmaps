using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace Ambacht.Common.Maps.Projections
{
    public class ProjectionCoordinateFilter : ICoordinateSequenceFilter
    {

        public ProjectionCoordinateFilter(Projection projection)
        {
            _projection = projection;
        }

        private readonly Projection _projection;

        public bool Done => false;
        public bool GeometryChanged => true;
        public void Filter(CoordinateSequence seq, int i)
        {
            var lng = (float)seq.GetX(i);
            var lat = (float)seq.GetY(i);
            var projected = _projection.Project(new LatLng(lat, lng));
            seq.SetX(i, projected.X);
            seq.SetY(i, projected.Y);
        }
    }
}
