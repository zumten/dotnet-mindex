using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZumtenSoft.Mindex.MappingCriterias;
using ZumtenSoft.Mindex.Mappings;

namespace ZumtenSoft.Mindex.Indexes
{
    [DebuggerDisplay(@"\{TableIndex " + nameof(Name) + @"={" + nameof(Name) + @"}\}")]
    public class TableIndex<TRow, TSearch> : TableRowCollection<TRow, TSearch>
    {
        public string Name => String.Join("_", _sortMappings.Select(x => x.Name));
        private readonly ITableMapping<TRow, TSearch>[] _sortMappings;
        private readonly BinarySearchTable<TRow> _rootResult;

        public TableIndex(TRow[] items, ITableMapping<TRow, TSearch>[] sortMappings)
            : base(SortRows(items, sortMappings))
        {
            _sortMappings = sortMappings;
            _rootResult = new BinarySearchTable<TRow>(Rows);
        }

        private static TRow[] SortRows(TRow[] items, IReadOnlyCollection<ITableMapping<TRow, TSearch>> sortColumns)
        {
            if (sortColumns.Count == 0)
                return items;
            var sortedRows = items.AsEnumerable();
            foreach (var criteria in sortColumns)
                sortedRows = criteria.Sort(sortedRows);
            return sortedRows.ToArray();
        }

        public override TRow[] Search(IReadOnlyCollection<ITableCriteriaForMapping<TRow, TSearch>> criterias)
        {
            if (_sortMappings.Length == 0)
                return base.Search(criterias);

            var binaryResult = _rootResult;
            var remainingCriterias = criterias.ToList();
            for (var indexSortColumn = 0;
                binaryResult.IsSearchable && indexSortColumn < _sortMappings.Length;
                indexSortColumn++)
            {
                var sortColumn = _sortMappings[indexSortColumn];
                var indexRemainingColumn = remainingCriterias.FindIndex(x => x.Mapping == sortColumn);
                if (indexRemainingColumn >= 0)
                {
                    var criteria = remainingCriterias[indexRemainingColumn];
                    var reducedResult = criteria.Reduce(binaryResult);
                    if (reducedResult == null)
                    {
                        binaryResult = new BinarySearchTable<TRow>(binaryResult.Segments, false);
                    }
                    else
                    {
                        binaryResult = reducedResult;
                        remainingCriterias.RemoveAt(indexRemainingColumn);
                    }
                }
                else
                {
                    binaryResult = new BinarySearchTable<TRow>(binaryResult.Segments, false);
                }
            }

            return FilterRowsWithCustomExpression(binaryResult, remainingCriterias);
        }

        public TableIndexScore<TRow, TSearch> GetScore(IReadOnlyCollection<ITableCriteriaForMapping<TRow, TSearch>> criterias)
        {
            TableCriteriaScore score = TableCriteriaScore.Initial;
            var remainingCriterias = criterias.ToList();
            for (var indexSortColumn = 0; score.CanContinue && indexSortColumn < _sortMappings.Length; indexSortColumn++)
            {
                var sortColumn = _sortMappings[indexSortColumn];
                var indexRemainingColumn = remainingCriterias.FindIndex(x => x.Mapping == sortColumn);
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