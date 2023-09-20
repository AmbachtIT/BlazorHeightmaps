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

        public static Vector3 ToVector3(this Vector2 v, float z) => new(v.X, v.Y, z);

    }
}
