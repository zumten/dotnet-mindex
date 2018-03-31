using System;
using System.Collections.Generic;

namespace ZumtenSoft.Mindex.Utilities
{
    public static class ArrayUtilities<T>
    {
        public static readonly T[] EmptyArray = new T[0];
        public static readonly ArraySegment<T> EmptySegment = new ArraySegment<T>(EmptyArray);

        public static int TotalCount(IReadOnlyList<T[]> input)
        {
            int result = 0;
            int length = input.Count;
            for (int i = 0; i < length; i++)
                result += input[i].Length;
            return result;
        }

        public static int TotalCount(ArraySegment<T>[] input)
        {
            int result = 0;
            int length = input.Length;
            for (int i = 0; i < length; i++)
                result += input[i].Count;
            return result;
        }

        public static T[] Flatten(T[][] input)
        {
            var result = new T[TotalCount(input)];
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

        public static T[] Flatten(ArraySegment<T>[] input)
        {
            T[] result = new T[TotalCount(input)];
            int position = 0;
            int length = input.Length;
            for (int iSegment = 0; iSegment < length; iSegment++)
            {
                var segment = input[iSegment];
                Array.Copy(segment.Array ?? throw new InvalidOperationException(), segment.Offset, result, position, segment.Count);
                position += segment.Count;
            }

            return result;
        }

        public static T[] Flatten(ArraySegment<T>[] input, Func<T, bool> predicate)
        {
            T[] result = new T[TotalCount(input)];
            int position = 0;
            for (int iSegment = 0; iSegment < input.Length; iSegment++)
            {
                var segment = input[iSegment];
                var array = segment.Array;
                var end = segment.Offset + segment.Count;
                for (int i = segment.Offset; i < end; i++)
                {
                    if (array != null)
                    {
                        T row = array[i];
                        if (predicate(row))
                            result[position++] = row;
                    }
                }
            }

            if (result.Length != position)
                Array.Resize(ref result, position);
            return result;
        }
    }
}
