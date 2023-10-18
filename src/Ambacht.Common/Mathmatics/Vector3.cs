using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
  public struct Vector3<T> where T : IFloatingPoint<T>
  {

    public Vector3() { }

    public Vector3(T x, T y, T z)
    {
      X = x;
      Y = y;
      Z = z;
    }

    public Vector3(T v)
    {
      X = v;
      Y = v;
    }

    public T X { get; set; }

    public T Y { get; set; }

    public T Z { get; set; }




    public static Vector3<T> operator +(Vector3<T> v1, Vector3<T> v2) => new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

    public static Vector3<T> operator -(Vector3<T> v1, Vector3<T> v2) => new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

    public static Vector3<T> operator *(Vector3<T> v1, T v2) => new(v1.X * v2, v1.Y * v2, v1.Z * v2);
    public static Vector3<T> operator /(Vector3<T> v1, T v2) => new(v1.X / v2, v1.Y / v2, v1.Z / v2);


  }
}
