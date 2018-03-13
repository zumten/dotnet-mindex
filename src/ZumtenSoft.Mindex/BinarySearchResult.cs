using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZumtenSoft.Mindex
{
    public class BinarySearchResult<TRow> : IReadOnlyCollection<TRow>
    {
        private readonly IReadOnlyList<ArraySegment<TRow>> _segments;
        public bool CanSearch { get; }
        public int Count => _segments.Sum(s => s.Count);

        public BinarySearchResult(TRow[] items)
        {
            _segments = new[] { new ArraySegment<TRow>(items) };
            CanSearch = true;
        }

        public BinarySearchResult(IReadOnlyList<ArraySegment<TRow>> segments, bool canSearch)
        {
            CanSearch = canSearch;
            _segments = segments;
        }

        public BinarySearchResult<TRow> ReduceIn<TColumn>(Func<TRow, TColumn> getColumn, TColumn[] values, IComparer<TColumn> comparer)
        {
            List<ArraySegment<TRow>> result = new List<ArraySegment<TRow>>();
            foreach (var key in values)
                foreach (var segment in _segments)
                    result.Add(SearchRange(segment, getColumn, key, key, comparer));

            return new BinarySearchResult<TRow>(result, true);
        }

        public BinarySearchResult<TRow> ReduceRange<TColumn>(Func<TRow, TColumn> getColumn, TColumn start, TColumn end, IComparer<TColumn> comparer)
        {
            List<ArraySegment<TRow>> result = new List<ArraySegment<TRow>>();
            foreach (var segment in _segments)
                result.Add(SearchRange(segment, getColumn, start, end, comparer));
                
            return new BinarySearchResult<TRow>(result, true);
        }

        public IEnumerator<TRow> GetEnumerator()
        {
            foreach (var segment in _segments)
            {
                int end = segment.Offset + segment.Count;
                TRow[] array = segment.Array ?? Array.Empty<TRow>();
                for (int i = segment.Offset; i < end; i++)
                    yield return array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static ArraySegment<TRow> SearchRange<TCompared>(ArraySegment<TRow> array, Func<TRow, TCompared> getCompared, TCompared start, TCompared end, IComparer<TCompared> comparer)
        {
            if (array.Array == null)
                throw new ArgumentNullException();

            var searchStart = InternalBinarySearch(array.Array, getCompared, array.Offset, array.Count, start, comparer, 1);
            var searchEnd = InternalBinarySearch(array.Array, getCompared, searchStart, array.Offset + array.Count - searchStart, end, comparer, -1);
            if (searchStart >= searchEnd)
                return new ArraySegment<TRow>(Array.Empty<TRow>());

            return new ArraySegment<TRow>(array.Array, searchStart, searchEnd - searchStart);
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