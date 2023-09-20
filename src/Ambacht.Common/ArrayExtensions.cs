using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Ambacht.Common
{
    public static class ArrayExtensions
    {
        public static T GetValue<T>(this T[,] array, Tuple<int, int> coords) => array[coords.Item1, coords.Item2];


        public static IEnumerable<(int, int)> GetIndices<T>(this T[,] array)
        {
            var width = array.GetLength(0);
            var height = array.GetLength(1);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    yield return (x, y);
                }
            }
        }

        public static IEnumerable<(int, int)> GetNeighbours<T>(this T[,] array, int x, int y)
        {
            var width = array.GetLength(0);
            var height = array.GetLength(1);
            if (x > 0)
            {
                yield return (x - 1, y);
            }

            if (x < width - 1)
            {
                yield return (x + 1, y);
            }

            if (y > 0)
            {
                yield return (x, y - 1);
            }

            if (y < height - 1)
            {
                yield return (x, y + 1);
            }
        }

        public static IEnumerable<List<(int, int)>> FloodFill<T>(this T[,] array, Func<T, T, bool> areEqual = null)
        {
            if (areEqual == null)
            {
                areEqual = (t1, t2) => t1.Equals(t2);
            }
            var width = array.GetLength(0);
            var height = array.GetLength(1);
            var visited = new bool[width, height];
            foreach (var (x, y) in array.GetIndices())
            {
                if (visited[x, y])
                {
                    continue;
                }

                yield return array.FloodFill(x, y, visited, areEqual);
            }
        }



        private static List<(int, int)> FloodFill<T>(this T[,] array, int x, int y, bool[, ] visited, Func<T, T, bool> areEqual = null)
        {
            var result = new List<(int, int)>();
            var queue = new Queue<(int, int)>();
            queue.Enqueue((x, y));
            var item = array[x, y];
            visited[x, y] = true;
            while (queue.Any())
            {
                (x, y) = queue.Dequeue();
                result.Add((x, y));
                foreach (var (nx, ny) in array.GetNeighbours(x, y))
                {
                    if (visited[nx, ny])
                    {
                        continue;
                    }
                    visited[nx, ny] = true;
                    if (item.Equals(array[nx, ny]))
                    {
                        queue.Enqueue((nx, ny));
                    }
                }
            }
            return result;
        }


        public static T TryGet<T>(this IList<T> list, int? index)
        {
            if (index == null || index.Value >= list.Count)
            {
                return default;
            }

            return list[index.Value];
        }

    }
}
