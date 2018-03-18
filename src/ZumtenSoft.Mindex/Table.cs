using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Columns;
using ZumtenSoft.Mindex.Indexes;

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

        protected TableColumnByValue<TRow, TSearch, TColumn> MapSearchCriteria<TColumn>(
            Expression<Func<TSearch, SearchCriteria<TColumn>>> getSearchValue,
            Expression<Func<TRow, TColumn>> getColumnValue)
        {
            return MapSearchCriteria(getSearchValue, getColumnValue, Comparer<TColumn>.Default, EqualityComparer<TColumn>.Default);
        }

        protected TableColumnByValue<TRow, TSearch, TColumn> MapSearchCriteria<TColumn, TComparer>(
            Expression<Func<TSearch, SearchCriteria<TColumn>>> getSearchValue,
            Expression<Func<TRow, TColumn>> getColumnValue,
            TComparer hybridComparer) where TComparer : IComparer<TColumn>, IEqualityComparer<TColumn>
        {
            return MapSearchCriteria(getSearchValue, getColumnValue, hybridComparer, hybridComparer);
        }

        protected TableColumnByValue<TRow, TSearch, TColumn> MapSearchCriteria<TColumn>(
            Expression<Func<TSearch, SearchCriteria<TColumn>>> getSearchValue,
            Expression<Func<TRow, TColumn>> getColumnValue,
            IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer)
        {
            var column = new TableColumnByValue<TRow, TSearch, TColumn>(_indexes.DefaultIndex.Rows, getColumnValue, getSearchValue, comparer, equalityComparer);
            _columns.Add(column);
            return column;
        }

        protected TableColumnByPredicate<TRow, TSearch, TColumn> MapMultiValuesSearchCriteria<TColumn>(Expression<Func<TSearch, SearchCriteriaByValue<TColumn>>> getColumnCriteria, Expression<Func<TRow, TColumn, bool>> predicate, bool isUnion = false)
        {
            var column = new TableColumnByPredicate<TRow, TSearch, TColumn>(getColumnCriteria, predicate, isUnion);
            _columns.Add(column);
            return column;
        }

        protected TableIndexConfigurator<TRow, TSearch> ConfigureIndex()
        {
            return new TableIndexConfigurator<TRow, TSearch>(_indexes, _columns);
        }

        public IEnumerable<TRow> Search(TSearch search, TableIndex<TRow, TSearch> index = null)
        {
            var criterias = _columns.ExtractCriterias(search);

            if (index != null)
                return index.Search(criterias);

            var bestIndex = _indexes.GetBestIndex(criterias);
            if (bestIndex == null)
                return BinarySearchResult<TRow>.EmptyArray;
            return bestIndex.Search(criterias);
        }

        public IEnumerable<TRow> Search(IEnumerable<TSearch> criterias)
        {
            foreach (var criteria in criterias)
                foreach (var result in Search(criteria))
                    yield return result;
        }

        public TableIndexScore<TRow, TSearch>[] EvaluateIndexes(TSearch search)
        {
            var criterias = _columns.ExtractCriterias(search);
            return _indexes.EvaluateIndexes(criterias);
        }
    }
}