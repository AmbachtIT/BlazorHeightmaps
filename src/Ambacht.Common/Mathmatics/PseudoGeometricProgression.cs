using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common.Mathmatics
{

    /// <summary>
    /// The pseudo-geometric progression is a scale the plays nicely with decimal numbers.
    ///
    /// Each number in the progression is either 2 or 2.5 times as large as the previous number.
    /// This is a useful property, because the numbers align nicely with round decimal numbers.
    /// It is the same progression used in euro coins and notes:
    ///
    /// ....0.1, 0.2, 0.5, 1, 2, 5, 10, 20, 50, etc...
    ///
    /// 
    /// </summary>
    public static class PseudoGeometricProgression<T> where T: IFloatingPoint<T>, ILogarithmicFunctions<T>, IExponentialFunctions<T>
    {

        /// <summary>
        /// Gets the largest number in the progression that is equal to or smaller than the specified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetLowerOrEqual(T value)
        {
            var (baseIndex, digit) = GetLowerAndEqualBaseAndDigit(value);
            return FromBaseAndDigit(baseIndex, digit);
        }

        /// <summary>
        /// Gets the largest number in the progression that is equal to or smaller than the specified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static (T, T) GetLowerAndEqualBaseAndDigit(T value)
        {
            var baseIndex = T.Floor(T.Log10(value));
            var baseMultiplier = GetBaseMultiplier(baseIndex);
            if(value >= Five * baseMultiplier)
            {
                return (baseIndex, Five);
            }
            if (value >= Two * baseMultiplier)
            {
                return (baseIndex, Two);
            }

            return (baseIndex, One);
        }

        /// <summary>
        /// Gets the largest number in the progression that is equal to or smaller than the specified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T FromBaseAndDigit(T baseIndex, T digit) => GetBaseMultiplier(baseIndex) * digit;


        public static T GetPrevious(T value)
        {
            var (baseIndex, digit) = GetLowerAndEqualBaseAndDigit(value);
            (baseIndex, digit) = GetPrevious(baseIndex, digit);
            return FromBaseAndDigit(baseIndex, digit);
        }

        private static (T, T) GetPrevious(T baseIndex, T digit)
        {
            if (digit >= Five)
            {
                return (baseIndex, Two);
            }
            if (digit >= Two)
            {
                return (baseIndex, One);
            }

            return (baseIndex - One, Five);
        }


        public static T GetNext(T value)
        {
            var (baseIndex, digit) = GetLowerAndEqualBaseAndDigit(value);
            (baseIndex, digit) = GetNext(baseIndex, digit);
            return FromBaseAndDigit(baseIndex, digit);
        }

        public static (T, T) GetNext(T baseIndex, T digit)
        {
            if (digit <= One)
            {
                return (baseIndex, Two);
            }
            if (digit <= Two)
            {
                return (baseIndex, Five);
            }

            return (baseIndex + One, One);
        }



        public static IEnumerable<T> ByBaseIndices(T fromB, T toB)
        {
            for (var b = fromB; b <= toB; b++)
            {
                foreach (var v in ByBaseIndex(b))
                {
                    yield return v;
                }
            }
        }

        public static IEnumerable<T> ByBaseIndex(T baseIndex)
        {
            var baseMultiplier = GetBaseMultiplier(baseIndex);
            foreach (var digit in Digits())
            {
                yield return digit * baseMultiplier;
            }
        }



        private static IEnumerable<T> Digits()
        {
            yield return One;
            yield return Two;
            yield return Five;
        }

        /// <summary>
        /// Returns
        /// </summary>
        /// <param name="baseIndex"></param>
        /// <returns></returns>
        public static T GetBaseMultiplier(T baseIndex) => T.Exp10(baseIndex);


        public static readonly T One = T.One;
        public static readonly T Two = One + One;
        public static readonly T Five = Two + Two + One;
        public static readonly T Ten = Two * Five;

    }
}
