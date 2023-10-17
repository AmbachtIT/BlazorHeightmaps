using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Intrinsics.Arm;

namespace Ambacht.Common.Maps.Tiles
{
    public class SlippyTileSet
    {

      private SlippyTileSet(string name)
      {
        this.Name = name;
      }


    public string Name { get; }

        public string UrlTemplate { get; set; }

        public int MinZoom { get; set; } = 1;
        public int MaxZoom { get; set; } = 19;
        public int TileSize { get; set; } = 256;


        public string Extension { get; set; } = "png";

        public static readonly SlippyTileSet OpenStreetMap = new SlippyTileSet(nameof(OpenStreetMap))
        {
            UrlTemplate = "https://a.tile.openstreetmap.org/level/tileX/tileY.png"
        };
        
        public static readonly SlippyTileSet OpenTopoMap = new SlippyTileSet(nameof(OpenTopoMap))
        {
            UrlTemplate = "https://b.tile.opentopomap.org/level/tileX/tileY.png",
            MaxZoom = 17
        };
        public static readonly SlippyTileSet TomTomDark = new SlippyTileSet(nameof(TomTomDark))
        {
            UrlTemplate = "https://a.api.tomtom.com/map/1/tile/basic/night/level/tileX/tileY.png?key=rA0fbGZ3M3n3H6qjwKQoKo9R6AQQWkbq"
        };

        public static readonly SlippyTileSet MapTilerBasic = new SlippyTileSet(nameof(MapTilerBasic))
        {
            UrlTemplate = "https://api.maptiler.com/maps/basic/level/tileX/tileY.png?key=7JzweQdG1iPd2ROnLGBT",
            TileSize = 512
        };

        public static readonly SlippyTileSet MapTilerToner = new SlippyTileSet(nameof(MapTilerToner))
        {
            UrlTemplate = "https://api.maptiler.com/maps/toner/level/tileX/tileY.png?key=7JzweQdG1iPd2ROnLGBT",
            TileSize = 512
        };

        public static readonly SlippyTileSet MapTilerStreets = new SlippyTileSet(nameof(MapTilerStreets))
        {
            UrlTemplate = "https://api.maptiler.com/maps/streets/level/tileX/tileY.png?key=7JzweQdG1iPd2ROnLGBT",
            TileSize = 512
        };
        

      public static readonly SlippyTileSet PdokLuchtFotoRgb25 = new SlippyTileSet(nameof(PdokLuchtFotoRgb25))
      {
        UrlTemplate = "https://service.pdok.nl/hwh/luchtfotorgb/wmts/v1_0/2023_quick_orthoHR/EPSG:3857/level/tileX/tileY.jpeg",
        Extension = "jpeg",
        MaxZoom = 21
      };


      public static IEnumerable<SlippyTileSet> All()
      {
        yield return OpenStreetMap;
        yield return OpenTopoMap;
        yield return TomTomDark;
        yield return MapTilerBasic;
        yield return MapTilerToner;
        yield return MapTilerStreets;
        yield return PdokLuchtFotoRgb25;
      }


    public IEnumerable<SlippyTileViewModel> GetVisibleTiles(SlippyMapView view)
        {
          if (view.TileSize == 0)
          {
            yield break;
          }
          if (view.TileSize != TileSize)
          {
              throw new InvalidOperationException("Tile size mismatch");
          }

            foreach (var tile in view.GetVisibleTiles())
            {
                var tileImage = new SlippyTileViewModel()
                {
                    Key = tile.ToString(),
                    Image = GetUrl(tile.X, tile.Y, tile.Z),
                    Coords = tile.Bounds().Center(),
                };
                tileImage.UpdateView(view);
                yield return tileImage;
            }
        }


    public string GetUrl(LatLng coords, int zoomLevel)
    {
      var tile = SlippyMath.LatLngToTile(coords, zoomLevel);
      return GetUrl((int) tile.X, (int) tile.Y, zoomLevel);
    }


    private string GetUrl(int x, int y, int zoomLevel)
        {
            int serverIndex = ((x + y) % 3) + 'a';
            return
                UrlTemplate
                    .Replace("https://a.", $"https://{(char)serverIndex}.")
                    .Replace("tileX", x.ToString())
                    .Replace("tileY", y.ToString())
                    .Replace("level", zoomLevel.ToString());
        }


        public override string ToString() => Name;

        public static SlippyTileSet ByName(string name) => All().SingleOrDefault(s => s.Name == name);
    }
}