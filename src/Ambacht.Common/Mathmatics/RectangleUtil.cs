using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
  public static class RectangleUtil
  {

    public static Rectangle<T> Cover<T>(IEnumerable<Vector2<T>> points) where T: IFloatingPoint<T>, IMinMaxValue<T>
    {
      var minX = T.MaxValue;
      var minY = T.MaxValue;
      var maxX = T.MinValue;
      var maxY = T.MinValue;

      foreach (var point in points)
      {
        minX = T.Min(minX, point.X);
        minY = T.Min(minY, point.Y);

        maxX = T.Max(maxX, point.X);
        maxY = T.Max(maxY, point.Y);
      }

      if (minX == T.MaxValue)
      {
        return Rectangle<T>.Empty;
      }

      return new(minX, minY, maxX - minX, maxY - minY);
    }

    public static Rectangle<T> Cover<T>(IEnumerable<Rectangle<T>> rects) where T : IFloatingPoint<T>, IMinMaxValue<T>
    {
      var minX = T.MaxValue;
      var minY = T.MaxValue;
      var maxX = T.MinValue;
      var maxY = T.MinValue;

      foreach (var rect in rects)
      {
        minX = T.Min(minX, rect.Left);
        minY = T.Min(minY, rect.Top);

        maxX = T.Max(maxX, rect.Right);
        maxY = T.Max(maxY, rect.Bottom);
      }

      if (minX == T.MaxValue)
      {
        return Rectangle<T>.Empty;
      }

      return new Rectangle<T>(minX, minY, maxX - minX, maxY - minY);
    }

  }
}
