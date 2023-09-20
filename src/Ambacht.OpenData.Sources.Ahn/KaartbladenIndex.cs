using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Ambacht.Common;
using Ambacht.Common.Mathmatics;
using CsvHelper;

namespace Ambacht.OpenData.Sources.Ahn
{
    public class KaartbladenIndex
    {

        public KaartbladenIndex()
        {
            using var stream = GetType().Assembly.GetManifestResourceStream("Ambacht.OpenData.Sources.Ahn.kaartbladen.txt");
            using var csv = new CsvReader(new StreamReader(stream), CultureInfo.InvariantCulture);
            foreach (var blad in csv.GetRecords<Kaartblad>())
            {
                var key = GetKey(blad.X, blad.Y);
                _bladenByCoords.Add(key, blad);
                _bladenById.Add(blad.Id, blad);
            }
        }

        public const int BladWidth = 5000;
        public const int BladHeight = 6250;
        public const int TileSizePx = 500;

        private Dictionary<(int, int), Kaartblad> _bladenByCoords = new();
        private Dictionary<string, Kaartblad> _bladenById = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rd">The position in RD coordinates</param>
        /// <returns>The AHN map sheet id, or null if the coordinate is out of bounds</returns>
        public Kaartblad GetBlad(Vector2 rd)
        {
            var key = GetKey(rd.X, rd.Y);
            return _bladenByCoords.TryGet(key);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>The AHN map sheet id, or null if the coordinate is out of bounds</returns>
        public Kaartblad GetBlad(string id)
        {
            return _bladenById.TryGet(id);
        }

        private (int, int) GetKey(float x, float y)
        {
            return ((int)(x / BladWidth), (int)(y / BladHeight));
        }

        public AhnTile GetTile(AhnRasterDataset set, Vector2 rd)
        {
            var sheet = GetBlad(rd);
            var local = rd - new Vector2(sheet.X, sheet.Y);
            var tileSizeMeters = TileSizePx * set.MetersPerPixel;
            local /= tileSizeMeters;
            var x = (int)local.X;
            var y = (int)local.Y;
            return new AhnTile()
            {
                Sheet = sheet.Id,
                X = x,
                Y = y,
                Bounds = new (sheet.X + x * tileSizeMeters, sheet.Y + y * tileSizeMeters, tileSizeMeters, tileSizeMeters)
            };
        }


    }


    public record class Kaartblad
    {
        public string Id { get; init; }
        public int X { get; init; }
        public int Y { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
    }
}
