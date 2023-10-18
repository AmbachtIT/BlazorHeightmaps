using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
    public static class MathExtensions
    {

        public static bool IsANumber<T>(this T? value) where T : struct, IFloatingPoint<T>
        {
            if (value.HasValue)
            {
                if (!T.IsNaN(value.Value))
                {
                    return true;
                }
            }
            return false;
        }


        public static Vector2 ToVector2(this Vector3 v) => new(v.X, v.Y);

        public static Vector2<T> ToVector2<T>(this Vector3<T> v) where T: IFloatingPoint<T> => new(v.X, v.Y);

    public static Vector3<T> ToVector3<T>(this Vector2<T> v, T z) where T : IFloatingPoint<T> => new(v.X, v.Y, z);

    }
}
