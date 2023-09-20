using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
    public record struct Percentage<T> : IFormattable
        where T: IFloatingPoint<T>, IFormattable
    {

        private T _value;

        public T PercentValue => _value * Hundred;

        public T FractionValue => _value;


        public static Percentage<T> FromPercentage(T percentage) => new Percentage<T>()
        {
            _value = percentage / Hundred
        };

        public static Percentage<T> FromFraction(T fraction) => new Percentage<T>()
        {
            _value = fraction
        };

        private static T Two = T.One + T.One;
        private static T Five = Two + Two + T.One;
        private static T Ten = Two * Five;
        private static T Hundred = Ten * Ten;

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return PercentValue.ToString(format, formatProvider) + "%";
        }
    }
}
