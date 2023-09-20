using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Intrinsics.Arm;

namespace Ambacht.Common.Maps.Tiles
{
    public class SlippyTileSet
    {
        public string UrlTemplate { get; set; }

        public int MinZoom { get; set; } = 1;
        public int MaxZoom { get; set; } = 19;
        public int TileSize { get; set; } = 256;

        public static readonly SlippyTileSet OpenStreetMap = new SlippyTileSet()
        {
            UrlTemplate = "https://a.tile.openstreetmap.org/level/tileX/tileY.png"
        };
        
        public static readonly SlippyTileSet OpenTopoMap = new SlippyTileSet()
        {
            UrlTemplate = "https://b.tile.opentopomap.org/level/tileX/tileY.png"
        };
        public static readonly SlippyTileSet TomTomDark = new SlippyTileSet()
        {
            UrlTemplate = "https://a.api.tomtom.com/map/1/tile/basic/night/level/tileX/tileY.png?key=rA0fbGZ3M3n3H6qjwKQoKo9R6AQQWkbq"
        };

        public static readonly SlippyTileSet MapTilerBasic = new SlippyTileSet()
        {
            UrlTemplate = "https://api.maptiler.com/maps/basic/level/tileX/tileY.png?key=7JzweQdG1iPd2ROnLGBT",
            TileSize = 512
        };

        public static readonly SlippyTileSet MapTilerToner = new SlippyTileSet()
        {
            UrlTemplate = "https://api.maptiler.com/maps/toner/level/tileX/tileY.png?key=7JzweQdG1iPd2ROnLGBT",
            TileSize = 512
        };

        public static readonly SlippyTileSet MapTilerStreets = new SlippyTileSet()
        {
            UrlTemplate = "https://api.maptiler.com/maps/streets/level/tileX/tileY.png?key=7JzweQdG1iPd2ROnLGBT",
            TileSize = 512
        };
        
        public static readonly SlippyTileSet MapTilerLight = new SlippyTileSet()
        {
            UrlTemplate = "https://api.maptiler.com/maps/light/level/tileX/tileY.png?key=7JzweQdG1iPd2ROnLGBT",
            TileSize = 512
        };

        
        
        
        public IEnumerable<SlippyTileViewModel> GetVisibleTiles(SlippyMapView view)
        {
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
                    Coords = tile.Bounds().NorthWest,
                };
                tileImage.UpdateView(view);
                yield return tileImage;
            }
        }


        private string GetUrl(int x, int y, int zoomLevel)
        {
            int serverIndex = ((x + y) % 3) + (int)'a';
            return
                UrlTemplate
                    .Replace("https://a.", $"https://{(char)serverIndex}.")
                    .Replace("tileX", x.ToString())
                    .Replace("tileY", y.ToString())
                    .Replace("level", zoomLevel.ToString());
        }

    }
}