using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
    public record class Stats<T> where T: IFloatingPointIeee754<T>
    {

        public int Count { get; set; }

        public T Total { get; internal set; } = T.Zero;

        public T Average { get; internal set; } = T.NaN;

        public T Variance { get; internal set; } = T.NaN;

        public T StandardDeviation { get; internal set; } = T.NaN;

        public T Min { get; internal set; } = T.NaN;

        public T Max { get; internal set; } = T.NaN;



        public static Stats<T> FromValues(IEnumerable<T> values)
        {
            var builder = new StatBuilder<T>();
            foreach (var value in values)
            {
                if (!T.IsNaN(value))
                {
                    builder.AddValue(value);
                }
            }

            return builder.Build();
        }

    }
}
