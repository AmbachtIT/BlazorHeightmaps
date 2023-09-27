using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.OpenData.Sources.Ahn
{


	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// These are the AHN4 download links for chart area 31HN1:
	/// https://service.pdok.nl/rws/ahn/atom/downloads
	/// - AHN4 0.5m raster DSM: https://service.pdok.nl/rws/ahn/atom/downloads/dsm_05m/R_31HN1.tif
	/// - AHN4 0.5m raster DTM: https://service.pdok.nl/rws/ahn/atom/downloads/dtm_05m/M_31HN1.tif
	/// - AHN4 0.5m map sheet index: https://service.pdok.nl/rws/ahn/atom/downloads/dsm_05m/kaartbladindex.json
	/// - AHN4 5m raster DSM: https://service.pdok.nl/rws/ahn/atom/downloads/dsm_5m/R_31HN1.tif
	/// - AHN4 5m raster DTM: https://service.pdok.nl/rws/ahn/atom/downloads/dtm_5m/M_31HN1.tif
	/// - AHN4 5m map sheet index: https://service.pdok.nl/rws/ahn/atom/downloads/dsm_5m/kaartbladindex.json
	/// 
	/// These are the AHN3 download links for chart area 31HN1:
	/// - AHN3 0.5m raster DSM: https://ns_hwh.fundaments.nl/hwh-ahn/AHN3/DSM_50cm/R_31HN1.zip
	/// - AHN3 0.5m raster DTM: https://ns_hwh.fundaments.nl/hwh-ahn/AHN3/DTM_50cm/M_31HN1.zip
	/// - AHN3 5m raster DSM: https://ns_hwh.fundaments.nl/hwh-ahn/AHN3/DSM_5m/R5_31HN1.zip
	/// - AHN3 5m raster DSM: https://ns_hwh.fundaments.nl/hwh-ahn/AHN3/DTM_5m/M5_31HN1.zip
	/// - AHN3 raw lidar cloud: https://ns_hwh.fundaments.nl/hwh-ahn/AHN3/LAZ/C_31HN1.LAZ
	/// </remarks>
	public abstract class AhnRasterDataset
    {

        /// <summary>
        /// 3 or 4
        /// </summary>
        public abstract int Version { get; }

        public AhnResolution Resolution { get; set; }

        public AhnLayer Layer { get; set; }

        public string GetDownloadLink(string kaartblad)
        {
            var prefix = Layer == AhnLayer.Dsm ? "R" : "M";
            if (Resolution == AhnResolution.Res_5m)
            {
	            prefix += "5";
            }
            var filename = $"{prefix}_{kaartblad}.{Extension}";
            return GetBaseUrl() + filename;
        }


        public float MetersPerPixel => Resolution == AhnResolution.Res_50cm ? 0.5f : 5f;


        protected abstract string GetBaseUrl();

        protected abstract string FormatLayer();

        protected abstract string FormatResolution();

        protected abstract string Extension { get; }


        public static readonly AhnRasterDataset Ahn4_Dtm_50cm = new Ahn4RasterDataset()
        {
            Layer = AhnLayer.Dtm,
            Resolution = AhnResolution.Res_50cm,
        };
        public static readonly AhnRasterDataset Ahn4_Dsm_50cm = new Ahn4RasterDataset()
        {
            Layer = AhnLayer.Dsm,
            Resolution = AhnResolution.Res_50cm,
        };

        public static readonly AhnRasterDataset Ahn3_Dtm_50cm = new Ahn3RasterDataset()
        {
            Layer = AhnLayer.Dtm,
            Resolution = AhnResolution.Res_50cm,
        };
        public static readonly AhnRasterDataset Ahn3_Dsm_50cm = new Ahn3RasterDataset()
        {
            Layer = AhnLayer.Dsm,
            Resolution = AhnResolution.Res_50cm,
        };

        public static readonly AhnRasterDataset Ahn3_Dtm_5m = new Ahn3RasterDataset()
        {
	        Layer = AhnLayer.Dtm,
	        Resolution = AhnResolution.Res_5m,
        };
        public static readonly AhnRasterDataset Ahn3_Dsm_5m = new Ahn3RasterDataset()
        {
	        Layer = AhnLayer.Dsm,
	        Resolution = AhnResolution.Res_5m,
        };

		public override string ToString() => $"ahn{Version}-{FormatLayer()}-{FormatResolution()}";

        public static AhnRasterDataset? Get(int version, AhnLayer layer, AhnResolution resolution)
        {
	        return (version, layer, resolution) switch
	        {
		        (4, AhnLayer.Dtm, AhnResolution.Res_50cm) => Ahn4_Dtm_50cm,
		        (4, AhnLayer.Dsm, AhnResolution.Res_50cm) => Ahn4_Dsm_50cm,

		        (3, AhnLayer.Dtm, AhnResolution.Res_50cm) => Ahn3_Dtm_50cm,
		        (3, AhnLayer.Dsm, AhnResolution.Res_50cm) => Ahn3_Dsm_50cm,
				(3, AhnLayer.Dtm, AhnResolution.Res_5m) => Ahn3_Dtm_5m,
		        (3, AhnLayer.Dsm, AhnResolution.Res_5m) => Ahn3_Dsm_5m,

                _ => null
	        };
        }
    }

    public class Ahn3RasterDataset : AhnRasterDataset
    {
        public override int Version => 3;

        protected override string Extension => "zip";

        protected override string GetBaseUrl() => $"https://ns_hwh.fundaments.nl/hwh-ahn/AHN3/{FormatLayer()}_{FormatResolution()}/";

        protected override string FormatLayer() => Layer.ToString().ToUpperInvariant();

        protected override string FormatResolution() => Resolution switch
        {
            AhnResolution.Res_50cm => "50cm",
            AhnResolution.Res_5m => "5m",
            _ => throw new NotImplementedException()
        };

	}

	public class Ahn4RasterDataset : AhnRasterDataset
    {
        public override int Version => 4;

        protected override string GetBaseUrl() => $"https://service.pdok.nl/rws/ahn/atom/downloads/{FormatLayer()}_{FormatResolution()}/";

        protected override string FormatLayer() => Layer.ToString().ToLowerInvariant();

        protected override string FormatResolution() => Resolution switch
        {
            AhnResolution.Res_50cm => "05m",
            AhnResolution.Res_5m => "5m",
            _ => throw new NotImplementedException()
        };

        protected override string Extension => "tif";


    }

    // AHN4:
    // DSM
    // https://service.pdok.nl/rws/ahn/atom/downloads/dsm_05m/kaartbladindex.json
    // https://service.pdok.nl/rws/ahn/atom/downloads/dsm_05m/R_02EZ1.tif
    //
    // DTM: 
    // https://service.pdok.nl/rws/ahn/atom/downloads/dtm_05m/kaartbladindex.json
    // https://service.pdok.nl/rws/ahn/atom/downloads/dtm_05m/M_01GN2.tif

    // AHN3
    // DSM
    // https://ns_hwh.fundaments.nl/hwh-ahn/AHN3/DSM_50cm/R_31HN1.zip
    // https://ns_hwh.fundaments.nl/hwh-ahn/AHN3/DSM_5m/R5_31HN1.zip
    //
    // DTM
    // https://ns_hwh.fundaments.nl/hwh-ahn/AHN3/DTM_50cm/M_31HN1.zip


    public enum AhnResolution {
        Res_50cm,
        Res_5m
    }

    public enum AhnLayer
    {
        /// <summary>
        /// Digital surface model, excluding buildings
        /// </summary>
        Dsm,
        /// <summary>
        /// Digital terrain model, including buildings etc.
        /// </summary>
        Dtm
    }


}
