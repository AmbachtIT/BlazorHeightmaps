using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{
    public class Monotonicity
    {

        public bool IsStrict { get; }
        public bool IsIncreasing { get; }

        private Monotonicity(bool isStrict, bool isIncreasing)
        {
            IsStrict = isStrict;
            IsIncreasing = isIncreasing;
        }

        public bool Matches<T>(IEnumerable<T> values) where T : struct, IComparable<T>
        {
            T? previous = null;
            foreach (var value in values)
            {
                if (previous is { } previousValue)
                {
                    var comparison = previousValue.CompareTo(value);
                    if (comparison != 0 && (comparison < 0 != IsIncreasing))
                    {
                        return false;
                    }
                    if (comparison == 0 && IsStrict)
                    {
                        return false;
                    }

                }
                previous = value;
            }

            return true;
        }

        public static Monotonicity GetMonotonicity<T>(IEnumerable<T> values) where T : struct, IComparable<T>
        {
            foreach (var monotonicity in All())
            {
                if (monotonicity.Matches(values))
                {
                    return monotonicity;
                }
            }

            return null;
        }

        private static IEnumerable<Monotonicity> All()
        {
            yield return StrictlyIncreasing;
            yield return StrictlyDecreasing;
            yield return Increasing;
            yield return Decreasing;
        }

        public static Monotonicity StrictlyIncreasing = new Monotonicity(true, true);
        public static Monotonicity StrictlyDecreasing = new Monotonicity(true, false);
        public static Monotonicity Increasing = new Monotonicity(false, true);
        public static Monotonicity Decreasing = new Monotonicity(false, false);

    }
}
