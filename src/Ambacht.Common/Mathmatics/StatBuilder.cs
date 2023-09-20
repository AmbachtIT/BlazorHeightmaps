using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
    public class StatBuilder<T> where T: IFloatingPointIeee754<T>
    {

        private readonly List<T> _values = new();

        public StatBuilder<T> AddValue(T value)
        {
            _values.Add(value);
            return this;
        }

        public Stats<T> Build()
        {
            var result = new Stats<T>()
            {
                Count = _values.Count,
            };
            var count = T.Zero;
            foreach (var value in _values)
            {
                count++;
                result.Total += value;
                result.Min = T.MinNumber(result.Min, value);
                result.Max = T.MaxNumber(result.Max, value);
            }

            if (result.Count > 0)
            {
                result.Average = result.Total / count;
                var sumOfSquares = T.Zero; 
                foreach (var value in _values)
                {
                    var diff = value - result.Average;
                    sumOfSquares += diff * diff;
                }

                result.Variance = sumOfSquares / count;
                result.StandardDeviation = T.Sqrt(result.Variance);
            }

            return result;
        }

        public StatBuilder<T> AddValues(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                AddValue(value);
            }

            return this;
        }
    }
}
