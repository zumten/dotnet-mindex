using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ZumtenSoft.Mindex.Utilities
{
    public static class ArrayUtilities
    {
        public static int Count<T>(IReadOnlyList<T[]> input)
        {
            int result = 0;
            int length = input.Count;
            for (int i = 0; i < length; i++)
                result += input[i].Length;
            return result;
        }

        public static int Count<T>(IReadOnlyList<ArraySegment<T>> input)
        {
            int result = 0;
            int length = input.Count;
            for (int i = 0; i < length; i++)
                result += input[i].Count;
            return result;
        }

        public static T[] Flatten<T>(T[][] input)
        {
            var result = new T[Count(input)];
            var length = input.Length;
            var position = 0;
            for (int i = 0; i < length; i++)
            {
                var segment = input[i];
                Array.Copy(segment, 0, result, position, segment.Length);
                position += segment.Length;
            }

            return result;
        }

        public static T[] Flatten<T>(ArraySegment<T>[] input)
        {
            T[] result = new T[Count(input)];
            int position = 0;
            int length = input.Length;
            for (int iSegment = 0; iSegment < length; iSegment++)
            {
                var segment = input[iSegment];
                Array.Copy(segment.Array, segment.Offset, result, position, segment.Count);
                position += segment.Count;
            }

            return result;
        }

        public static T[] Flatten<T>(ArraySegment<T>[] input, Func<T, bool> predicate)
        {
            T[] accumulator = new T[Count(input)];
            int position = 0;
            for (int iSegment = 0; iSegment < input.Length; iSegment++)
            {
                var segment = input[iSegment];
                var array = segment.Array;
                var end = segment.Offset + segment.Count;
                for (int i = segment.Offset; i < end; i++)
                {
                    T row = array[i];
                    if (predicate(row))
                        accumulator[position++] = row;
                }
            }

            T[] result = new T[position];
            Array.Copy(accumulator, result, position);
            return result;
        }
    }
}
