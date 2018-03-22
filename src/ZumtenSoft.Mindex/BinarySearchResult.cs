using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Utilities;

namespace ZumtenSoft.Mindex
{
    public class BinarySearchResult<TRow> : IReadOnlyCollection<TRow>
    {
        public static readonly TRow[] EmptyArray = new TRow[0];
        private static readonly ArraySegment<TRow> EmptySegment = new ArraySegment<TRow>(EmptyArray);
        public ArraySegment<TRow>[] Segments { get; }
        public bool CanSearch { get; }
        public int Count => Segments.Sum(s => s.Count);

        public BinarySearchResult(TRow[] items)
        {
            Segments = new[] { new ArraySegment<TRow>(items) };
            CanSearch = true;
        }

        public BinarySearchResult(ArraySegment<TRow>[] segments, bool canSearch)
        {
            CanSearch = canSearch;
            Segments = segments;
        }

        public BinarySearchResult<TRow> ReduceIn<TColumn>(Func<TRow, TColumn> getColumn, TColumn[] values, IComparer<TColumn> comparer)
        {
            List<ArraySegment<TRow>> result = new List<ArraySegment<TRow>>();
            for (int iValue = values.Length - 1; iValue >= 0; iValue--)
            {
                TColumn value = values[iValue];
                for (int iSegment = Segments.Length - 1; iSegment >= 0; iSegment--)
                {
                    var range = SearchRange(Segments[iSegment], getColumn, value, value, comparer);
                    if (range != EmptySegment)
                        result.Add(range);
                }
            }

            return new BinarySearchResult<TRow>(result.ToArray(), true);
        }

        public BinarySearchResult<TRow> ReduceRange<TColumn>(Func<TRow, TColumn> getColumn, TColumn start, TColumn end, IComparer<TColumn> comparer)
        {
            List<ArraySegment<TRow>> result = new List<ArraySegment<TRow>>();
            foreach (var segment in Segments)
            {
                var range = SearchRange(segment, getColumn, start, end, comparer);
                if (range != EmptySegment)
                    result.Add(range);
            }
                
            return new BinarySearchResult<TRow>(result.ToArray(), false);
        }

        public BinarySearchResult<TRow> ReduceRangeByValue<TColumn>(Func<TRow, TColumn> getColumn, TColumn start, TColumn end, IComparer<TColumn> comparer)
        {
            List<ArraySegment<TRow>> result = new List<ArraySegment<TRow>>();
            foreach (var segment in Segments)
            {
                var range = SearchRange(segment, getColumn, start, end, comparer);
                if (range != EmptySegment)
                    SplitByValue(result, range, getColumn, comparer);
            }

            return new BinarySearchResult<TRow>(result.ToArray(), true);
        }

        public static BinarySearchResult<TRow> Merge(IReadOnlyList<BinarySearchResult<TRow>> results)
        {
            if (results.Count == 0)
                return new BinarySearchResult<TRow>(EmptySegment.Array);
            if (results.Count == 1)
                return results[0];
            return new BinarySearchResult<TRow>(results.SelectMany(r => r.Segments).ToArray(), false);
        }

        public IEnumerator<TRow> GetEnumerator()
        {
            foreach (var segment in Segments)
            {
                int end = segment.Offset + segment.Count;
                TRow[] array = segment.Array;
                if (array != null)
                    for (int i = segment.Offset; i < end; i++)
                        yield return array[i];
            }
        }

        public TRow[] ToArray()
        {
            return ArrayUtilities.Flatten(Segments);
        }

        public TRow[] Filter(Func<TRow, bool> predicate)
        {
            return ArrayUtilities.Flatten(Segments, predicate);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static void SplitByValue<TCompared>(List<ArraySegment<TRow>> result, ArraySegment<TRow> initialSegment, Func<TRow, TCompared> getCompared, IComparer<TCompared> comparer)
        {
            TRow[] array = initialSegment.Array;
            int searchStart = initialSegment.Offset;
            int searchEnd = searchStart + initialSegment.Count;

            while (searchStart < searchEnd)
            {
                var searchSplit = InternalBinarySearch(array, getCompared, searchStart, searchEnd - searchStart, getCompared(array[searchStart]), comparer, -1);
                result.Add(new ArraySegment<TRow>(array, searchStart, searchSplit - searchStart));
                searchStart = searchSplit;
            }
        }

        private static ArraySegment<TRow> SearchRange<TCompared>(ArraySegment<TRow> initialSegment, Func<TRow, TCompared> getCompared, TCompared valueStart, TCompared valueEnd, IComparer<TCompared> comparer)
        {
            TRow[] array = initialSegment.Array;
            if (array == null)
                throw new ArgumentNullException();

            int searchStart = InternalBinarySearch(array, getCompared, initialSegment.Offset, initialSegment.Count, valueStart, comparer, 1);
            int searchEnd = InternalBinarySearch(array, getCompared, searchStart, initialSegment.Offset + initialSegment.Count - searchStart, valueEnd, comparer, -1);
            if (searchStart >= searchEnd)
                return EmptySegment;

            return new ArraySegment<TRow>(array, searchStart, searchEnd - searchStart);
        }

        private static int InternalBinarySearch<T, TCompared>(T[] array, Func<T, TCompared> getCompared, int index, int length, TCompared value, IComparer<TCompared> comparer, int fallback)
        {
            int num1 = index;
            int num2 = index + length - 1;
            while (num1 <= num2)
            {
                int index1 = num1 + (num2 - num1 >> 1);
                int num3 = comparer.Compare(getCompared(array[index1]), value);
                if (num3 == 0)
                    num3 = fallback;
                if (num3 < 0)
                    num1 = index1 + 1;
                else
                    num2 = index1 - 1;
            }

            return num1;
        }
    }
}