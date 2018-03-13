using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex
{
    public class TableIndex<TRow, TSearch> : TableRowCollection<TRow, TSearch>
    {
        private readonly IReadOnlyCollection<ITableColumn<TRow, TSearch>> _sortColumns;

        public TableIndex(TRow[] items, IReadOnlyCollection<ITableColumn<TRow, TSearch>> columns, IReadOnlyCollection<ITableColumn<TRow, TSearch>> sortColumns)
            : base(SortRows(items, sortColumns), columns)
        {
            _sortColumns = sortColumns;
        }

        private static TRow[] SortRows(IEnumerable<TRow> items, IReadOnlyCollection<ITableColumn<TRow, TSearch>> sortColumns)
        {
            foreach (var criteria in sortColumns)
                items = criteria.Sort(items);
            return items.ToArray();
        }

        public override IEnumerable<TRow> Search(TSearch search)
        {
            if (_sortColumns.Count == 0)
                return base.Search(search);

            var binaryResult = new BinarySearchResult<TRow>(Rows);
            List<ITableColumn<TRow, TSearch>> processedColumns = new List<ITableColumn<TRow, TSearch>>();
            foreach (var column in _sortColumns)
            {
                if (!binaryResult.CanSearch || !column.Reduce(search, ref binaryResult))
                    break;
                processedColumns.Add(column);
            }

            var remainingColumns = Columns.Except(processedColumns).ToList();
            return FilterRowsWithCustomExpression(binaryResult, search, remainingColumns);
        }

        public float GetScore(TSearch search)
        {
            float score = 0;
            foreach (var column in _sortColumns)
            {
                var columnScore = column.GetScore(search);
                score += columnScore.Item1;
                if (!columnScore.Item2)
                    break;
            }

            return score;
        }
    }

}