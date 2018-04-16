using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ZumtenSoft.Mindex.Utilities;

namespace ZumtenSoft.Mindex
{
    [DebuggerDisplay(@"\{ArraySegmentCollection Segments={Segments.Length}, TotalCount={Count}\}")]
    public class ArraySegmentCollection<TRow> : IReadOnlyCollection<TRow>
    {
        public ArraySegment<TRow>[] Segments { get; }
        public bool IsSearchable { get; }
        public int Count => ArrayUtilities<TRow>.TotalCount(Segments);

        public ArraySegmentCollection(TRow[] items)
        {
            Segments = new[] { new ArraySegment<TRow>(items) };
            IsSearchable = true;
        }

        public ArraySegmentCollection(ArraySegment<TRow>[] segments, bool isSearchable)
        {
            IsSearchable = isSearchable;
            Segments = segments;
        }

        /// <summary>
        /// Apply a binary search on each segment to push the reduction of the result set one step further.
        /// </summary>
        /// <typeparam name="TColumn">Type of the searchable field</typeparam>
        /// <param name="getColumn">Function to extract the searchable value from the row</param>
        /// <param name="valuesToSearch">List of values that must be matched in order to preserve a row</param>
        /// <param name="comparer">Comparer for this type of column</param>
        /// <returns></returns>
        public ArraySegmentCollection<TRow> ReduceByValues<TColumn>(Func<TRow, TColumn> getColumn, TColumn[] valuesToSearch, IComparer<TColumn> comparer)
        {
            ValidateSearchable();
            List<ArraySegment<TRow>> result = new List<ArraySegment<TRow>>();
            for (int iValue = valuesToSearch.Length - 1; iValue >= 0; iValue--)
            {
                TColumn value = valuesToSearch[iValue];
                for (int iSegment = Segments.Length - 1; iSegment >= 0; iSegment--)
                {
                    var range = SearchRange(Segments[iSegment], getColumn, value, value, comparer);
                    if (range.Count >= 0)
                        result.Add(range);
                }
            }

            return new ArraySegmentCollection<TRow>(result.ToArray(), true);
        }

        /// <summary>
        /// Apply a binary search on each segment to extract values from "start" to "end".
        /// Since the range can include multiple values, this kind of search will break the index from searching further.
        /// To preserve this functionnality, each value can be split into a different ArraySegment.
        /// </summary>
        /// <typeparam name="TColumn">Type of the searchable field</typeparam>
        /// <param name="getColumn">Function to extract the searchable value from the row</param>
        /// <param name="start">Minimum value to search</param>
        /// <param name="end">Maximum value to search</param>
        /// <param name="comparer">Comparer for this type of column</param>
        /// <param name="preserveSearchability">Preserve the searchability by splitting every value into a different ArraySegment</param>
        /// <returns></returns>
        public ArraySegmentCollection<TRow> ReduceByRange<TColumn>(Func<TRow, TColumn> getColumn, TColumn start, TColumn end, IComparer<TColumn> comparer, bool preserveSearchability = false)
        {
            ValidateSearchable();
            List<ArraySegment<TRow>> result = new List<ArraySegment<TRow>>();
            foreach (var segment in Segments)
            {
                var range = SearchRange(segment, getColumn, start, end, comparer);
                if (range.Count >= 0)
                {
                    if (preserveSearchability)
                        SplitByValue(result, range, getColumn, comparer);
                    else
                        result.Add(range);
                }
            }

            return new ArraySegmentCollection<TRow>(result.ToArray(), preserveSearchability);
        }
        
        public IEnumerator<TRow> GetEnumerator()
        {
            foreach (var segment in Segments)
            {
                int end = segment.Offset + segment.Count;
                TRow[] array = segment.Array
                    ?? throw new InvalidOperationException("ArraySegment is missing Array");
                for (int i = segment.Offset; i < end; i++)
                    yield return array[i];
            }
        }

        /// <summary>
        /// Equivalent of .ToArray(), but optimized to work with ArraySegments.
        /// </summary>
        public TRow[] Materialize()
        {
            return ArrayUtilities<TRow>.Flatten(Segments);
        }

        /// <summary>
        /// Equivalent of .Where(predicate).ToArray(), but optimized to work with ArraySegments.
        /// </summary>
        public TRow[] Materialize(Func<TRow, bool> predicate)
        {
            return ArrayUtilities<TRow>.Flatten(Segments, predicate);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ValidateSearchable()
        {
            if (!IsSearchable)
                throw new Exception(nameof(ArraySegmentCollection<TRow>) + " has been marked as non-searchable");
        }

        private static void SplitByValue<TCompared>(List<ArraySegment<TRow>> result, ArraySegment<TRow> initialSegment, Func<TRow, TCompared> getCompared, IComparer<TCompared> comparer)
        {
            TRow[] array = initialSegment.Array
                           ?? throw new InvalidOperationException("ArraySegment is missing Array");
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
                return ArrayUtilities<TRow>.EmptySegment;

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