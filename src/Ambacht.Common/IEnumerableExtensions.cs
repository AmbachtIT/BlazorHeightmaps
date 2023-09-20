using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common
{
    public static class IEnumerableExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> items, int batchSize)
        {
            var batch = new List<T>();
            foreach (var item in items)
            {
                batch.Add(item);
                if (batch.Count >= batchSize)
                {
                    yield return batch;
                    batch = new List<T>();
                }
            }

            if (batch.Count > 0)
            {
                yield return batch;
            }
        }


        public static void RemoveRange<T>(this List<T> list, int count)
        {
            if (list.Count <= count)
            {
                return;
            }
            list.RemoveRange(count, list.Count - count);
        }

        public static void RemoveRange(this IList list, int count)
        {
            if (list.Count <= count)
            {
                return;
            }

            for (var i = list.Count - 1; i >= count; i--)
            {
                list.RemoveAt(i);
            }
        }

        public static void RemoveRange<T>(this IList<T> list, int count)
        {
            if (list.Count <= count)
            {
                return;
            }

            for (var i = list.Count - 1; i >= count; i--)
            {
                list.RemoveAt(i);
            }
        }

        public static IEnumerable<(T, T)> AllPairs<T>(this IList<T> items)
        {
            for (var i1 = 0; i1 < items.Count; i1++)
            {
                for (var i2 = i1 + 1; i2 < items.Count; i2++)
                {
                    yield return (items[i1], items[i2]);
                }
            }
        }


        public static IEnumerable<T> NotNullOrNan<T>(this IEnumerable<T?> items) where T : struct, IFloatingPoint<T>
        {
            return items.Where(i => i.HasValue).Select(i => i.Value).Where(v => !T.IsNaN(v));
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> items) where T : struct
        {
            return items.Where(i => i.HasValue).Select(i => i.Value);
        }

        public static IEnumerable<T> SafeEnum<T>(this IEnumerable<T> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    yield return item;
                }
            }
        }


        public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> items)
        {
            var i = 0;
            foreach (var item in items)
            {
                yield return (item, i++);
            }
        }


        public static IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item)
        {
            return items.Concat(new[] {item});
        }

        /// <summary>
        /// Enumerates two IEnumerables in parallel. Will yield default(T) if enum for enums that have no more values.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="items1"></param>
        /// <param name="items2"></param>
        /// <returns></returns>
        public static IEnumerable<(T1, T2)> EnumerateWith<T1, T2>(this IEnumerable<T1> items1, IEnumerable<T2> items2)
        {
            using var enum1 = items1.GetEnumerator();
            using var enum2 = items2.GetEnumerator();
            bool hasItems1 = enum1.MoveNext();
            bool hasItems2 = enum2.MoveNext();
            while (hasItems1 || hasItems2)
            {
                yield return (hasItems1 ? enum1.Current : default, hasItems2 ? enum2.Current : default);
                hasItems1 = enum1.MoveNext();
                hasItems2 = enum2.MoveNext();
            }
        }



        /// <summary>
        /// Enumerates two IEnumerables in parallel. Will yield default(T) if enum for enums that have no more values.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="items1"></param>
        /// <param name="items2"></param>
        /// <param name="items3"></param>
        /// <returns></returns>
        public static IEnumerable<(T1, T2, T3)> EnumerateWith<T1, T2, T3>(this IEnumerable<T1> items1, IEnumerable<T2> items2, IEnumerable<T3> items3)
        {
            using var enum1 = items1.GetEnumerator();
            using var enum2 = items2.GetEnumerator();
            using var enum3 = items3.GetEnumerator();
            bool hasItems1 = enum1.MoveNext();
            bool hasItems2 = enum2.MoveNext();
            bool hasItems3 = enum2.MoveNext();
            while (hasItems1 || hasItems2 || hasItems3)
            {
                yield return (hasItems1 ? enum1.Current : default, 
                              hasItems2 ? enum2.Current : default, 
                              hasItems3 ? enum3.Current : default);
                hasItems1 = enum1.MoveNext();
                hasItems2 = enum2.MoveNext();
                hasItems3 = enum3.MoveNext();
            }
        }




    }
}
