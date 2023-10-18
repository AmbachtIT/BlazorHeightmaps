using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ambacht.Common.Mathmatics
{
  public struct Vector2<T> : IEquatable<Vector2<T>> where T: IFloatingPoint<T> 
  {

    public Vector2() {}

    public Vector2(T x, T y)
    {
      X = x;
      Y = y;
    }

    public Vector2(T v)
    {
      X = v;
      Y = v;
    }

    public T X { get; set; }

    public T Y { get; set; }


    public static Vector2<T> operator +(Vector2<T> v1, Vector2<T> v2) => new(v1.X + v2.X, v1.Y + v2.Y);

    public static Vector2<T> operator -(Vector2<T> v1, Vector2<T> v2) => new(v1.X - v2.X, v1.Y - v2.Y);

    public static Vector2<T> operator *(Vector2<T> v1, T v2) => new(v1.X * v2, v1.Y * v2);
    public static Vector2<T> operator /(Vector2<T> v1, T v2) => new(v1.X / v2, v1.Y / v2);

    public static bool operator ==(Vector2<T> v1, Vector2<T> v2) => v1.X == v2.X && v1.Y == v2.Y;
    public static bool operator !=(Vector2<T> v1, Vector2<T> v2) => v1.X != v2.X || v1.Y != v2.Y;


    public Vector2<T2> Cast<T2>() where T2 : IFloatingPoint<T2> => new Vector2<T2>((T2)(object)X, (T2)(object)Y);


    public bool Equals(Vector2<T> other)
    {
      return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y);
    }

    public override bool Equals(object obj)
    {
      return obj is Vector2<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(X, Y);
    }

    public static readonly Vector2<T> Zero = new Vector2<T>(T.Zero, T.Zero);

    public static readonly Vector2<T> One = new Vector2<T>(T.One, T.One);


    public static readonly T Two = T.One + T.One;
  }





}
