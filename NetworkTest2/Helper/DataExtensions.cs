using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkTest2.Helper
{
    public static class DataExtensions
    {
        public static int InverseEndian(this int value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes);
        }

        public static uint InverseEndian(this uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            var array = enumerable.ToArray();
            var random = new Random();
            for (var i = 0; i < array.Length; i++)
            {
                var j = random.Next(0, array.Length);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
            return array;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, int maxChunksize)
        {
            var array = enumerable.ToArray();
            var result = new List<IEnumerable<T>>();

            var maxChunks = Convert.ToInt32(Math.Ceiling(array.Length / (double) maxChunksize));

            for (var i = 0; i < maxChunks; i++)
            {
                var startIndex = i * maxChunksize;
                var size = Math.Min(startIndex + maxChunksize, array.Length) % maxChunksize;

                var items = new T[size];
                for (var j = 0; j < size; j++)
                {
                    items[j] = array[startIndex + j];
                }
                result.Add(items);
            }

            return result;
        }
    }
}
