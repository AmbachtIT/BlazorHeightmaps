using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace Ambacht.Common.Maps.Kml
{
    public class KmlBuilder : IDisposable
    {
        private StreamWriter writer;
        private readonly string path;

        public KmlBuilder(string path, string title, string description)
        {
            this.path = path;
            Stream stream;
            if (path.EndsWith("kml"))
            {
                stream = File.Create(path);
            }
            else if (path.EndsWith("kmz"))
            {
                stream = File.Create(path.Replace(".kmz", ".kml.tmp"));
            }
            else
            {
                throw new InvalidOperationException("Invalid extension: " + path);
            }
            writer = new StreamWriter(stream);
            writer.WriteLine(
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"">
  <Document>
    <name>{0}</name>
    <description>{1}</description>",
                title, FormatDescription(description));
        }

        private string FormatDescription(string description)
        {
            if (description != null && (description.Contains('<') || description.Contains('>')))
            {
                return string.Format("<![CDATA[{0}]]>", description);
            }
            return description;
        }

        /*public void AddLegendLineStyles(Legend legend, double lineThickness)
        {
            for (int step = 0; step < legend.Steps; step++)
            {
                var value = legend.GetValue(step);
                AddLineStyle(legend.GetStyleId(value), legend.GetColor(value), lineThickness);
            }
        }*/

        /*public void AddLineStyle(string id, Color color, double thickness)
        {
            writer.WriteLine(
                @"    <Style id=""{0}"">
      <LineStyle>
        <color>{1}</color>
        <width>{2}</width>
      </LineStyle>
    </Style>", id, KmlHelper.FormatColor(color), thickness.ToString(numberFormat));
        }*/

        /*/// <summary>
        /// 
        /// </summary>
        /// <param name="id">style Id</param>
        /// <param name="color">Color of the road</param>
        /// <param name="outerColor">Color of the outer lines</param>
        /// <param name="physicalWidth">Physical width of the road in m</param>
        /// <param name="outerFraction">fraction of road that is colored with 'outerColor'</param>
        public void AddRoadLineStyle(string id, Color color, Color outerColor, double physicalWidth, double outerFraction)
        {
            writer.WriteLine(
                @"<Style id=""{0}"">
      <LineStyle>
        <color>{1}</color>
        <gx:physicalWidth>{2}</gx:physicalWidth>
        <gx:outerColor>{3}</gx:outerColor>
        <gx:outerWidth>{4}</gx:outerWidth>
      </LineStyle>
    </Style>",
                id,
                KmlHelper.FormatColor(color),
                physicalWidth.ToString(numberFormat),
                KmlHelper.FormatColor(outerColor),
                outerFraction.ToString(numberFormat));
        }*/

        /*public void AddPolygonStyle(string id, Color fillColor, Color strokeColor, double thickness)
        {
            writer.WriteLine(
                @"<Style id=""{0}"">
      <LineStyle>
        <color>{1}</color>
        <width>{2}</width>
      </LineStyle>
      <PolyStyle>
        <color>{3}</color>
      </PolyStyle>
    </Style>", id, KmlHelper.FormatColor(strokeColor), thickness.ToString(numberFormat), KmlHelper.FormatColor(fillColor));
        }*/

        public void AddPolygonStyle(string id, string fill, string stroke, double lineWidth)
        {
            writer.WriteLine(
                $@"<Style id=""{id}"">
      <PolyStyle>
        <color>{KmlHelper.FormatColor(fill)}</color>
      </PolyStyle>
      <LineStyle>
        <color>{KmlHelper.FormatColor(stroke)}</color>
        <width>{lineWidth.ToString(numberFormat)}</width>
      </LineStyle>
    </Style>");
        }

        public void AddLineStyle(string id, string stroke, double lineWidth)
        {
            writer.WriteLine(
                $@"<Style id=""{id}"">
      <LineStyle>
        <color>{KmlHelper.FormatColor(stroke)}</color>
        <width>{lineWidth.ToString(numberFormat)}</width>
      </LineStyle>
    </Style>");
        }


        public void AddIconStyle(string id, string iconHref)
        {
            writer.WriteLine(@"<Style id=""{0}"">
      <IconStyle><Icon><href>{1}</href> </Icon></IconStyle>
    </Style>", id, iconHref);
        }

        public void AddMarker(string title, string description, LatLng coords, string styleId = null)
        {
            writer.WriteLine(@"	<Placemark>
		<name>{0}</name>
		<description>{1}</description>", title, FormatDescription(description));
            if (styleId != null)
            {
                writer.WriteLine(@"		<styleUrl>#{0}</styleUrl>", styleId);
            }
            writer.WriteLine(@"		<Point>
		  <coordinates>{0}</coordinates>
		</Point>
	</Placemark>",
                             KmlHelper.FormatCoordinate(coords));
        }

        public void AddMarker(LatLngAltitude coords, string styleId = null)
        {
            writer.WriteLine(@"	<Placemark>@");
            if (styleId != null)
            {
                writer.WriteLine(@"		<styleUrl>#{0}</styleUrl>", styleId);
            }
            writer.WriteLine($@"		<Point>
            <altitudeMode>relativeToGround</altitudeMode>
		  <coordinates>{KmlHelper.FormatCoordinate(coords)}</coordinates>
		</Point>
	</Placemark>");
        }


        public void AddPolygon(string title, string description, Polygon polygon, string styleId = null, double altitude = 0)
        {
            writer.WriteLine(
                @"<Placemark>
      <name>{0}</name>
      <description>{1}</description>", title, FormatDescription(description));
            if (styleId != null)
            {
                writer.WriteLine($@"      <styleUrl>#{styleId}</styleUrl>");
            }
            writer.WriteLine(@"      <Polygon>
      <outerBoundaryIs>
        <LinearRing>
          <tessellate>1</tessellate>
          <coordinates>{0}</coordinates>
        </LinearRing>
      </outerBoundaryIs>", string.Join(" ", polygon.Points.Select(c => KmlHelper.FormatCoordinate(c, altitude)).ToArray()));
            foreach (var hole in polygon.Holes)
            {
                writer.WriteLine(@"      <innerBoundaryIs>
        <LinearRing>
          <tessellate>1</tessellate>
          <coordinates>{0}</coordinates>
        </LinearRing>
      </innerBoundaryIs>", string.Join(" ", hole.Points.Select(c => KmlHelper.FormatCoordinate(c, altitude)).ToArray()));
            }
            writer.WriteLine(@"    </Polygon>
    </Placemark>");
        }

        public void AddCross(LatLngAltitude pos, float sizeMeters, string styleId)
        {
            writer.WriteLine(@"<Placemark>");
            if (styleId != null)
            {
                writer.WriteLine($"\t<styleUrl>#{styleId}</styleUrl>");
            }



            AddLine2(pos.Translate(new(-sizeMeters, 0, 0)), pos.Translate(new(sizeMeters, 0, 0)));
            AddLine2(pos.Translate(new(0, -sizeMeters, 0)), pos.Translate(new(0, sizeMeters, 0)));
            AddLine2(pos.Translate(new(0, 0, -sizeMeters)), pos.Translate(new(0, 0, sizeMeters)));

            writer.WriteLine($"</Placemark>");
        }

        public void AddLine(LatLngAltitude from, LatLngAltitude to, string styleId)
        {
            writer.WriteLine(@"<Placemark>");
            if (styleId != null)
            {
                writer.WriteLine($"\t<styleUrl>#{styleId}</styleUrl>");
            }
            AddLine2(from, to);
            writer.WriteLine(@"</Placemark>");
        }

        private void AddLine2(LatLngAltitude from, LatLngAltitude to)
        {
            writer.WriteLine($"\t<LineString>");
            writer.WriteLine($"\t\t<altitudeMode>relativeToGround</altitudeMode>");
            writer.WriteLine($"\t\t<coordinates>{KmlHelper.FormatCoordinate(from)} {KmlHelper.FormatCoordinate(to)}</coordinates>");
            writer.WriteLine($"\t</LineString>");
        }

        public void AddLineString(string title, string description, string styleId, params LatLng[] coords)
        {
            writer.WriteLine(
                @"<Placemark>
      <name>{0}</name>
      <description>{1}</description>", title, FormatDescription(description));
            if (styleId != null)
            {
                writer.WriteLine(@"      <styleUrl>#{0}</styleUrl>", styleId);
            }
            writer.WriteLine($"\t<LineString>");
            writer.WriteLine($"\t\t<altitudeMode>clampToGround</altitudeMode>");
            var formatted = string.Join(" ", coords.Select(c => KmlHelper.FormatCoordinate(c, 0)));
            writer.WriteLine($"\t\t<coordinates>{formatted}</coordinates>");
            writer.WriteLine($"\t</LineString>");
            writer.WriteLine($"</Placemark>");
        }



        public void Dispose()
        {
            if (writer != null)
            {
                writer.WriteLine(@"  </Document>
</kml>");
                writer.Dispose();
                writer = null;

                if (path.EndsWith(".kmz"))
                {
                    var tempFileName = path.Replace(".kmz", ".kml.tmp");
                    var zipped = ZipFile.Create(path);
                    zipped.BeginUpdate();
                    zipped.Add(tempFileName, "doc.kml");
                    zipped.CommitUpdate();
                    zipped.Close();
                    File.Delete(tempFileName);
                }
            }
        }

        private static readonly IFormatProvider numberFormat = CultureInfo.InvariantCulture.NumberFormat;


        public IDisposable StartFolder(string name)
        {
            writer.WriteLine("<Folder>");
            writer.WriteLine($" <name>{name}</name>");
            return new Finally(() =>
            {
                writer.WriteLine("</Folder>");
            });
        }
    }
}
