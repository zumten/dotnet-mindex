using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Indexes
{
    [DebuggerDisplay(@"\{TableIndex " + nameof(Name) + @"={" + nameof(Name) + @"}\}")]
    public class TableIndex<TRow, TSearch> : TableRowCollection<TRow, TSearch>
    {
        public string Name => String.Join("_", _sortColumns.Select(x => x.Name));
        private readonly ITableColumn<TRow, TSearch>[] _sortColumns;
        private readonly BinarySearchResult<TRow> _rootResult;

        public TableIndex(TRow[] items, ITableColumn<TRow, TSearch>[] sortColumns)
            : base(SortRows(items, sortColumns))
        {
            _sortColumns = sortColumns;
            _rootResult = new BinarySearchResult<TRow>(Rows);
        }

        private static TRow[] SortRows(TRow[] items, IReadOnlyCollection<ITableColumn<TRow, TSearch>> sortColumns)
        {
            if (sortColumns.Count == 0)
                return items;
            var sortedRows = items.AsEnumerable();
            foreach (var criteria in sortColumns)
                sortedRows = criteria.Sort(sortedRows);
            return sortedRows.ToArray();
        }

        public override IEnumerable<TRow> Search(IReadOnlyCollection<ITableCriteriaForColumn<TRow, TSearch>> criterias)
        {
            if (_sortColumns.Length == 0)
                return base.Search(criterias);

            var binaryResult = _rootResult;
            var remainingCriterias = criterias.ToList();
            for (var indexSortColumn = 0;
                binaryResult.CanSearch && indexSortColumn < _sortColumns.Length;
                indexSortColumn++)
            {
                var sortColumn = _sortColumns[indexSortColumn];
                var indexRemainingColumn = remainingCriterias.FindIndex(x => x.Column == sortColumn);
                if (indexRemainingColumn >= 0)
                {
                    var criteria = remainingCriterias[indexRemainingColumn];
                    var reducedResult = criteria.Reduce(binaryResult);
                    if (reducedResult == null)
                    {
                        binaryResult = new BinarySearchResult<TRow>(binaryResult.Segments, false);
                    }
                    else
                    {
                        binaryResult = reducedResult;
                        remainingCriterias.RemoveAt(indexRemainingColumn);
                    }
                }
                else
                {
                    binaryResult = new BinarySearchResult<TRow>(binaryResult.Segments, false);
                }
            }

            return FilterRowsWithCustomExpression(binaryResult, remainingCriterias);
        }

        public TableIndexScore<TRow, TSearch> GetScore(IReadOnlyCollection<ITableCriteriaForColumn<TRow, TSearch>> criterias)
        {
            TableCriteriaScore score = TableCriteriaScore.Initial;
            var remainingCriterias = criterias.ToList();
            for (var indexSortColumn = 0; score.CanContinue && indexSortColumn < _sortColumns.Length; indexSortColumn++)
            {
                var sortColumn = _sortColumns[indexSortColumn];
                var indexRemainingColumn = remainingCriterias.FindIndex(x => x.Column == sortColumn);
                if (indexRemainingColumn >= 0)
                {
                    var criteria = remainingCriterias[indexRemainingColumn];
                    score *= criteria.Score;
                    remainingCriterias.RemoveAt(indexRemainingColumn);
                }
                else
                {
                    score.CanContinue = false;
                }
            }

            return new TableIndexScore<TRow, TSearch>(this, score.Value);
        }
    }
}