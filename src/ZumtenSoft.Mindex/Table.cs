using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Columns;
using ZumtenSoft.Mindex.Indexes;
using ZumtenSoft.Mindex.Utilities;

namespace ZumtenSoft.Mindex
{
    public abstract class Table<TRow, TSearch>
    {
        private readonly TableColumnCollection<TRow, TSearch> _columns;
        public IReadOnlyCollection<ITableColumn<TRow, TSearch>> Columns => _columns;

        private readonly TableIndexCollection<TRow, TSearch> _indexes;

        protected Table(IReadOnlyCollection<TRow> rows)
        {
            _columns = new TableColumnCollection<TRow, TSearch>();
            _indexes = new TableIndexCollection<TRow, TSearch>(rows);
        }

        /// <summary>
        /// Map criteria to a column using the default comparer
        /// </summary>
        protected TableColumnByValue<TRow, TSearch, TColumn> MapCriteria<TColumn>(
            Expression<Func<TSearch, SearchCriteria<TColumn>>> getCriteriaValue,
            Expression<Func<TRow, TColumn>> getColumnValue)
        {
            return MapCriteria(getCriteriaValue, getColumnValue, Comparer<TColumn>.Default, EqualityComparer<TColumn>.Default);
        }

        /// <summary>
        /// Map criteria to a column using a comparer that supports both IComparer and IEqualityComparer (ex: StringComparer)
        /// </summary>
        protected TableColumnByValue<TRow, TSearch, TColumn> MapCriteria<TColumn, TComparer>(
            Expression<Func<TSearch, SearchCriteria<TColumn>>> getCriteriaValue,
            Expression<Func<TRow, TColumn>> getColumnValue,
            TComparer hybridComparer) where TComparer : IComparer<TColumn>, IEqualityComparer<TColumn>
        {
            return MapCriteria(getCriteriaValue, getColumnValue, hybridComparer, hybridComparer);
        }

        /// <summary>
        /// Map criteria to a column using both a comparer and an equality comparer.
        /// </summary>
        protected TableColumnByValue<TRow, TSearch, TColumn> MapCriteria<TColumn>(
            Expression<Func<TSearch, SearchCriteria<TColumn>>> getCriteriaValue,
            Expression<Func<TRow, TColumn>> getColumnValue,
            IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer)
        {
            var column = new TableColumnByValue<TRow, TSearch, TColumn>(_indexes.DefaultIndex.Rows, getColumnValue, getCriteriaValue, comparer, equalityComparer);
            _columns.Add(column);
            return column;
        }

        /// <summary>
        /// Map criteria to a column with multiple values and which cannot be indexed.
        /// This type of column supports only the criterias with values.
        /// </summary>
        protected TableColumnByPredicate<TRow, TSearch, TColumn> MapCriteriaToPredicate<TColumn>(Expression<Func<TSearch, SearchCriteriaByValue<TColumn>>> getColumnCriteria, Expression<Func<TRow, TColumn, bool>> columnPredicate, bool isUnion = false)
        {
            var column = new TableColumnByPredicate<TRow, TSearch, TColumn>(getColumnCriteria, columnPredicate, isUnion);
            _columns.Add(column);
            return column;
        }

        /// <summary>
        /// Start configuring an index using the builder pattern.
        /// </summary>
        protected TableIndexConfigurator<TRow, TSearch> ConfigureIndex()
        {
            return new TableIndexConfigurator<TRow, TSearch>(_indexes, _columns);
        }

        /// <summary>
        /// Search the table using the search object passed in parameter. The search will
        /// be executed using the specified index or the index that best matches the criterias.
        /// </summary>
        /// <param name="search">Search containing the list of criterias to apply</param>
        /// <param name="index">Optionally force an index</param>
        /// <returns></returns>
        public TRow[] Search(TSearch search, TableIndex<TRow, TSearch> index = null)
        {
            var criterias = _columns.ExtractCriterias(search);

            if (index != null)
                return index.Search(criterias);

            var bestIndex = _indexes.GetBestIndex(criterias);
            // Null is returned when any of the index find that this search will return
            // an empty result set.
            if (bestIndex == null)
                return BinarySearchResult<TRow>.EmptyArray;
            return bestIndex.Search(criterias);
        }

        public TRow[] Search(TSearch[] criterias, bool distinct = false)
        {
            switch (criterias.Length)
            {
                case 0: return BinarySearchResult<TRow>.EmptyArray;
                case 1: return Search(criterias[0]);
                default:
                {
                    if (distinct)
                    {
                        HashSet<TRow> rows = new HashSet<TRow>();
                        foreach (var criteria in criterias)
                        {
                            var result = Search(criteria);
                            var length = result.Length;
                            for (int i = 0; i < length; i++)
                                rows.Add(result[i]);
                        }

                        return rows.ToArray();
                    }
                    else
                    {
                        TRow[][] results = new TRow[criterias.Length][];
                        for (int i = 0; i < criterias.Length; i++)
                            results[i] = Search(criterias[i]);

                        return ArrayUtilities.Flatten(results);
                    }
                }
            }
        }

        public TableIndexScore<TRow, TSearch>[] EvaluateIndexes(TSearch search)
        {
            var criterias = _columns.ExtractCriterias(search);
            return _indexes.EvaluateIndexes(criterias);
        }
    }
}