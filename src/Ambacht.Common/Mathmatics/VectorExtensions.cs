using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
  public static class VectorExtensions
  {


    public static T Length<T>(this Vector2<T> v) where T : IFloatingPoint<T>, IFloatingPointIeee754<T>
    {
        return T.Sqrt(v.X * v.X + v.Y * v.Y);
    }

  }
}
