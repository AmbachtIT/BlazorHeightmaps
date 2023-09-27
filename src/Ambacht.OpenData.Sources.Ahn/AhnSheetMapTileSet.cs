using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common.Maps;
using Ambacht.Common.Maps.Tiles;
using Rectangle = Ambacht.Common.Mathmatics.Rectangle;

namespace Ambacht.OpenData.Sources.Ahn
{
	public class AhnSheetMapTileSet : IMapTileSet
	{

		public AhnSheetMapTileSet(AhnRasterDataset dataset)
		{
			_dataset = dataset;
		}

		private readonly AhnRasterDataset _dataset;
		private readonly KaartbladenIndex _index = new KaartbladenIndex();


		public IMapTile? GetTile(Vector2 coords)
		{
			var sheet = _index.GetBlad(coords);
			if (sheet == null)
			{
				return null;
			}

			return new AhnSheetTile()
			{
				Key = sheet.Id,
				Bounds = new Rectangle(sheet.X, sheet.Y, sheet.Width, sheet.Height),
				Crs = Crs.RdEpsg,
				Url = _dataset.GetDownloadLink(sheet.Id)
			};
		}

		public IEnumerable<IMapTile> GetTiles(Rectangle bounds)
		{
			var topLeft = _index.GetBlad( bounds.TopLeft());
			var bottomRight = _index.GetBlad( bounds.BottomRight());

			for (var y = topLeft.Y; y <= bottomRight.Y; y += KaartbladenIndex.BladHeightMeters)
			{
				for (var x = topLeft.X; x <= bottomRight.X; x += KaartbladenIndex.BladWidthMeters)
				{
					var tile = GetTile(new(x, y));
					if (tile != null)
					{
						yield return tile;
					}
				}
			}
		}
	}

	public class AhnSheetTile : IMapTile
	{
		public string Key { get; init; }
		public string Url { get; init; }
		public string Crs { get; init; }
		public Rectangle Bounds { get; init; }


		public override string ToString() => Key;
	}

}
