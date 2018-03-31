using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ZumtenSoft.Mindex.ColumnCriterias;

namespace ZumtenSoft.Mindex.Indexes
{
    [DebuggerDisplay(@"\{TableIndexCollection Count={" + nameof(Count) + @"}\}")]
    public class TableIndexCollection<TRow, TSearch> : List<TableIndex<TRow, TSearch>>
    {
        public TableRowCollection<TRow, TSearch> DefaultIndex { get; private set; }

        public TableIndexCollection(IReadOnlyCollection<TRow> rows)
        {
            DefaultIndex = new TableRowCollection<TRow, TSearch>(rows.ToArray());
        }

        public TableIndexScore<TRow, TSearch>[] EvaluateIndexes(
            IReadOnlyCollection<ITableCriteriaForColumn<TRow, TSearch>> criterias)
        {
            return this
                .Select(i => i.GetScore(criterias))
                .OrderBy(i => i.Score)
                .ToArray();
        }

        public TableRowCollection<TRow, TSearch> GetBestIndex(
            IReadOnlyCollection<ITableCriteriaForColumn<TRow, TSearch>> criterias)
        {
            if (Count == 0)
                return DefaultIndex;

            var bestMatch = this
                .Select(i => i.GetScore(criterias))
                .Aggregate((x, y) => x.Score < y.Score ? x : y);

            // If we receive a score of 0, it means one of the criteria is impossible.
            // Might as well notify we can return an empty result set.
            if (bestMatch.Score <= 0f)
                return null;

            // If no index was built, we will use the default rows collection
            return bestMatch.Index;
        }

        public new void Add(TableIndex<TRow, TSearch> index)
        {
            base.Add(index);
            // First index overrides the default collection to save memory
            DefaultIndex = DefaultIndex as TableIndex<TRow, TSearch> ?? index;
        }
    }
}