using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Maps.Projections
{
    public static class Wkt
    {

        public static readonly string Wgs84 = @"GEOGCS[""GCS_WGS_1984"",
     DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137,298.257223563]],
     PRIMEM[""Greenwich"",0],
     UNIT[""Degree"",0.0174532925199433]
]";



        public static readonly string Epgs4326 = @"GEOGCS[""WGS 84"",
    DATUM[""WGS_1984"",
        SPHEROID[""WGS 84"",6378137,298.257223563,
            AUTHORITY[""EPSG"",""7030""]],
        AUTHORITY[""EPSG"",""6326""]],
    PRIMEM[""Greenwich"",0,
        AUTHORITY[""EPSG"",""8901""]],
    UNIT[""degree"",0.0174532925199433,
        AUTHORITY[""EPSG"",""9122""]],
    AUTHORITY[""EPSG"",""4326""]]";

    }
}
