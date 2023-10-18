using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Mathmatics;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace Ambacht.Common.Maps.Projections
{
    /// <summary>
    /// Implements a RijksCoordinaten Driehoeksstelsel projection
    /// </summary>
    public class RijksDriehoeksProjection : Projection
    {

        public RijksDriehoeksProjection()
        {
            var csFact = new CoordinateSystemFactory();
            var ctFact = new CoordinateTransformationFactory();
            var wgs84 = GeographicCoordinateSystem.WGS84;
            var rdm = csFact.CreateFromWkt("PROJCS[\"Amersfoort / RD New\",GEOGCS[\"Amersfoort\",DATUM[\"Amersfoort\",SPHEROID[\"Bessel 1841\",6377397.155,299.1528128,AUTHORITY[\"EPSG\",\"7004\"]],TOWGS84[565.4171,50.3319,465.5524,-0.398957,0.343988,-1.87740,4.0725],AUTHORITY[\"EPSG\",\"6289\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4289\"]],PROJECTION[\"Oblique_Stereographic\"],PARAMETER[\"latitude_of_origin\",52.15616055555555],PARAMETER[\"central_meridian\",5.38763888888889],PARAMETER[\"scale_factor\",0.9999079],PARAMETER[\"false_easting\",155000],PARAMETER[\"false_northing\",463000],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"X\",EAST],AXIS[\"Y\",NORTH],AUTHORITY[\"EPSG\",\"28992\"]]");
            _transformation = ctFact.CreateFromCoordinateSystems(wgs84, rdm);
            _inverse = ctFact.CreateFromCoordinateSystems(rdm, wgs84);
        }


        private readonly ICoordinateTransformation _transformation;
        private readonly ICoordinateTransformation _inverse;


        public override Vector2<double> Project(LatLng pos)
        {
            var coord = new [] {pos.Longitude, pos.Latitude};
            var transformed = _transformation.MathTransform.Transform(coord);
            return new Vector2<double>(transformed[0], transformed[1]);
        }

        public override LatLng Invert(Vector2<double> pos)
        {
            var coord = new [] { pos.X, pos.Y };
            var transformed = _inverse.MathTransform.Transform(coord);
            return new LatLng(transformed[1], transformed[0]);
        }
    }

}
