﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps.Projections;
using Ambacht.Common.Mathmatics;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using Point = NetTopologySuite.Geometries.Point;
using Rectangle = Ambacht.Common.Mathmatics.Rectangle;

namespace Ambacht.Common.Maps
{
    public static class NTSExtensions
    {

        public static double GetAreaKM2(this NetTopologySuite.Geometries.Geometry geom)
        {
            return geom.GetAreaM2() / 1_000_000;
        }

        public static double GetAreaM2(this NetTopologySuite.Geometries.Geometry geom)
        {
            var scale = GetScale(geom);
            var area = geom.Area * scale * scale;
            if (area > EarthSurfaceAreaM2 / 2)
            {
                area = EarthSurfaceAreaM2 - area;
            }

            return area;
        }

        private static double GetScale(Geometry geometry)
        {
            switch (geometry.Factory.SRID)
            {
                case 3857:
                    return GetWebMercatorScale(geometry);
                default:
                    return 1;
            }
        }

        private static double GetWebMercatorScale(Geometry geometry)
        {
            var y = geometry.Centroid.Y;
            var latRadians = Math.Atan(Math.Exp(y / EarthRadius)) * 2 - Math.PI / 2;
            var lat = 180 * latRadians / Math.PI;
            return Math.Cos(latRadians);
        }

        



        public static void Project(this NetTopologySuite.Geometries.Geometry geom, Projections.Projection projection)  {
            geom.Apply(new ProjectionCoordinateFilter(projection));
        }


        public const double EarthSurfaceAreaM2 = 510e12;
        public const double EarthRadius = 6378137;


        public static double GetLengthKM(this NetTopologySuite.Geometries.Geometry geom)
        {
            return geom.GetLengthM() / 1_000;
        }


        public static double GetLengthM(this NetTopologySuite.Geometries.Geometry geom)
        {
            var scale = GetScale(geom);
            return geom.Length * scale;
        }



        public static double DistanceTo(this Point p1, Point p2)
        {
            return p1.Distance(p2) * GetScale(p1);
        }


        public static Vector2 ToVector2(this Point p)
        {
            return new Vector2((float) p.X, (float) p.Y);
        }

        public static Vector2 ToVector2(this Coordinate p)
        {
            return new Vector2((float)p.X, (float)p.Y);
        }

        public static Point ToPoint(this Vector2 v)
        {
            return new Point(v.X, v.Y);
        }


        public static IEnumerable<Vector2> GetBoundary(this Geometry geometry)
        {
            return geometry switch
            {
                NetTopologySuite.Geometries.Polygon poly => GetBoundary(poly),
                _ => throw new InvalidOperationException()
            };
        }


        public static IEnumerable<Vector2> GetBoundary(this NetTopologySuite.Geometries.Polygon polygon)
        {
            return GetBoundary(polygon.ExteriorRing);
        }

        public static IEnumerable<Vector2> GetBoundary(this NetTopologySuite.Geometries.LineString line)
        {
            return
                line
                    .Coordinates
                    .Select(c => new Vector2((float) c[0], (float) c[1]));
        }



        public static Polygon CreatePolygon(this Geometry geometry, Projection projection)
        {
            return geometry switch
            {
                NetTopologySuite.Geometries.Polygon poly => CreatePolygon(poly, projection),
                LineString line => new Polygon()
                {
                    Points = CreatePolygon(line, projection)
                },
                _ => throw new InvalidOperationException()
            };
        }

        public static IEnumerable<Polygon> ToMultiPolygon(this Geometry geometry, Projection projection)
        {
            return geometry switch
            {
                NetTopologySuite.Geometries.Polygon poly => new[] { CreatePolygon(poly, projection) },
                NetTopologySuite.Geometries.MultiPolygon poly => poly.Geometries.Select(g => g.CreatePolygon(projection)).ToArray(),
                LineString line => new [] { new Polygon()
                {
                    Points = CreatePolygon(line, projection)
                } },
                _ => throw new InvalidOperationException()
            };
        }


        public static Polygon CreatePolygon(this NetTopologySuite.Geometries.Polygon polygon, Projection projection)
        {
            return new Polygon()
            {
                Points = CreatePolygon(polygon.ExteriorRing, projection),
                Holes = polygon.InteriorRings.Select(line => new Polygon()
                {
                    Points = CreatePolygon(line, projection)
                }).ToList()
            };
        }

        public static List<LatLng> CreatePolygon(this LineString line, Projection projection)
        {
            return
                line
                    .Coordinates
                    .Select(projection.Project)
                    .ToList();
        }

        public static LatLng Project(this Projection projection, Coordinate coord)
        {
            return projection.Invert(new Vector2((float)coord.X, (float)coord.Y));
        }



        public static Rectangle GetBoundingRectangle(this Geometry geometry)
        {
            if (geometry == null)
            {
                return Rectangle.Empty;
            }

            return Rectangle.Cover(geometry.GetBoundary());
        }


        public static bool Contains(this Geometry geometry, Vector2 v) => geometry.Contains(new Point(v.X, v.Y));


    }
}