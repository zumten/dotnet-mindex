using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Indexes
{
    public class TableIndexCollection<TRow, TSearch>
    {
        private readonly List<TableIndex<TRow, TSearch>> _indexes;
        public TableRowCollection<TRow, TSearch> DefaultIndex { get; private set; }

        public TableIndexCollection(IReadOnlyCollection<TRow> rows)
        {
            _indexes = new List<TableIndex<TRow, TSearch>>();
            DefaultIndex = new TableRowCollection<TRow, TSearch>(rows.ToArray());
        }

        public TableRowCollection<TRow, TSearch> GetBestIndex(IReadOnlyCollection<ITableColumnCriteria<TRow, TSearch>> criterias)
        {
            if (_indexes.Count == 0)
                return DefaultIndex;

            var bestMatch = _indexes
                .Select(i => new { score = i.GetScore(criterias), index = i })
                .Aggregate((x, y) => x.score < y.score ? x : y);

            // If we receive a score of 0, it means one of the criteria is impossible.
            // Might as well notify we can return an empty result set.
            if (bestMatch.score <= 0f)
                return null;

            // If no index was built, we will use the default rows collection
            return bestMatch.index;
        }

        public void Add(TableIndex<TRow, TSearch> index)
        {
            _indexes.Add(index);
            // First index overrides the default collection to save memory
            DefaultIndex = DefaultIndex as TableIndex<TRow, TSearch> ?? index;
        }
    }
}
