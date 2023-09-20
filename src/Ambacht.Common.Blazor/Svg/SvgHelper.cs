using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Blazor.Svg
{
    public static class SvgHelper
    {


        /// <summary>
        /// This method downsamples data to limit the size of SVG polylines and polygons.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2> Downsample(this IList<Vector2> points, Vector2 size)
        {
            if (size.X <= 0 || size.Y <= 0)
            {
                yield break;
            }
            var samplesPerPixelX = (int)Math.Floor(points.Count / size.X);
            var samplesPerPixelY = (int)Math.Floor(points.Count / size.Y);
            var samplesPerPixel = Math.Min(samplesPerPixelX, samplesPerPixelY);
            if (samplesPerPixel <= 1)
            {
                // No need to downsample
                foreach (var point in points)
                {
                    yield return point;
                }
                yield break;
            }
            var sum = Vector2.Zero;
            var count = 0;

            foreach (var point in points)
            {
                sum += point;
                count++;
                if (count >= samplesPerPixel)
                {
                    yield return new Vector2(sum.X / count, sum.Y / count);
                    count = 0;
                    sum = Vector2.Zero;
                }
            }

            if (count > 1)
            {
                yield return new Vector2(sum.X / count, sum.Y / count);
            }
        }


        public static string ToSvgPoints(this IEnumerable<Vector2> points)
        {
            var builder = new StringBuilder();
            foreach (var point in points)
            {
                if (builder.Length > 0)
                {
                    builder.Append(' ');
                }

                builder.Append(point.X.ToString(neutral));
                builder.Append(',');
                builder.Append(point.Y.ToString(neutral));
            }

            return builder.ToString();
        }

        private static readonly IFormatProvider neutral = CultureInfo.InvariantCulture;

    }
}
