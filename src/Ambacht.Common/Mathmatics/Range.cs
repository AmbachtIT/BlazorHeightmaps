using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
    public record struct Range<T> where T: IComparable<T>
    {
        public Range(T min, T max)
        {
            if (min.IsGreater(max))
            {
                this.Min = max;
                this.Max = min;
            }
            else
            {
                this.Min = min;
                this.Max = max;
            }
        }

        public T Min { get; }

        public T Max { get; }

        [Pure()]
        public bool Contains(T value)
        {
            if (value.IsGreater(Max))
            {
                return false;
            }

            if (value.IsLess(Min))
            {
                return false;
            }

            return true;
        }

        public bool IsEmpty => Min.Equals(Max);


        public override string ToString() => $"[{Min}, {Max}]";
    }


    public static class RangeExtensions
    {
        public static Range<T> GetRange<T>(this IEnumerable<T> items) where T : IComparable<T>
        {
            bool first = true;
            T min = default;
            T max = default;
            foreach (var item in items)
            {
                if (first)
                {
                    min = item;
                    max = item;
                    first = false;
                }
                else
                {
                    min = min.Min(item);
                    max = max.Max(item);
                }
            }

            return new Range<T>(min, max);
        }


        public static T Lerp<T>(this Range<T> range, T alpha) where T : IFloatingPoint<T>
        {
            return range.Min + (range.Max - range.Min) * alpha;
        }


        public static Range<T> PadFraction<T>(this Range<T> range, T amount) where T : IFloatingPoint<T>
        {
            amount = amount.Clamp(T.Zero, T.One);
            var halfAmount = amount * (range.Max - range.Min) / (T.One + T.One);
            return new(range.Min + halfAmount, range.Max - halfAmount);
        }


        public static Range<T> GetEvenlySpacedPart<T>(this Range<T> range, int index, int count) where T : IFloatingPoint<T>
        {
            if (index >= count || count <= 0)
            {
                return range;
            }
            var bandSize = (range.Max - range.Min) / T.Parse(count.ToString(), CultureInfo.InvariantCulture);
            var start = range.Min + bandSize * T.Parse(index.ToString(), CultureInfo.InvariantCulture);
            return new Range<T>(start, start + bandSize);
        }


        public static Range<float> ToFloat(this Range<double> range)
        {
            return new Range<float>((float)range.Min, (float)range.Max);
        }

    }
}
