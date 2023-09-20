using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
    public class OrderOfMagnitude : IComparable<OrderOfMagnitude>
    {  
        private OrderOfMagnitude(int size)
        {
            Size = size;
            Base = Math.Pow(10, Size);
        }


        public int Size { get;  }
        public double Base { get; }


        public double GetRelativeValue(double value) => value / Base;


        public static OrderOfMagnitude Get(double value)
        {
            if (value < 0)
            {
                return Get(-value);
            }
            if (value == 0)
            {
                return null;
            }

            var log10 = Math.Log10(value);
            return new OrderOfMagnitude((int)Math.Floor(log10));
        }

        public static OrderOfMagnitude GetBounded(double value, int min, int max)
        {
            var result = Get(value);
            if (result == null)
            {
                return new OrderOfMagnitude(min);
            }

            if (result.Size < min)
            {
                return new OrderOfMagnitude(min);
            }

            if (result.Size > max)
            {
                return new OrderOfMagnitude(max);
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as OrderOfMagnitude;
            if (other != null)
            {
                return other.Size == this.Size;
            }

            return false;
        }

        public override int GetHashCode() => Size.GetHashCode();

        public int CompareTo(OrderOfMagnitude other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var sizeComparison = Size.CompareTo(other.Size);
            if (sizeComparison != 0) return sizeComparison;
            return Base.CompareTo(other.Base);
        }
    }
}
