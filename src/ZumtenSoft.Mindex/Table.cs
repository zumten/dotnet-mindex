using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex
{
    public abstract class Table<TRow, TSearch>
    {
        public TableRowCollection<TRow, TSearch> DefaultIndex { get; private set; }

        private readonly List<ITableColumn<TRow, TSearch>> _columns;
        public IReadOnlyCollection<ITableColumn<TRow, TSearch>> Columns => _columns;

        private readonly List<TableIndex<TRow, TSearch>> _indexes;
        public IReadOnlyCollection<TableIndex<TRow, TSearch>> Indexes => _indexes;

        protected Table(IReadOnlyCollection<TRow> rows)
        {
            DefaultIndex = new TableRowCollection<TRow, TSearch>(rows.ToArray(), Columns);
            _columns = new List<ITableColumn<TRow, TSearch>>();
            _indexes = new List<TableIndex<TRow, TSearch>>();
        }

        protected TableColumn<TRow, TSearch, TColumn> MapSearchCriteria<TColumn>(
            Expression<Func<TSearch, SearchCriteria<TColumn>>> getSearchValue,
            Expression<Func<TRow, TColumn>> getColumnValue,
            IComparer<TColumn> comparer = null)
        {
            var column = new TableColumn<TRow, TSearch, TColumn>(getColumnValue, getSearchValue, comparer ?? Comparer<TColumn>.Default);
            _columns.Add(column);
            return column;
        }

        protected TableMultiValuesColumn<TRow, TSearch, TColumn> MapMultiValuesSearchCriteria<TColumn>(Expression<Func<TSearch, SearchCriteriaByValue<TColumn>>> getColumnCriteria, Expression<Func<TRow, TColumn, bool>> predicate)
        {
            var column = new TableMultiValuesColumn<TRow, TSearch, TColumn>(getColumnCriteria, predicate);
            _columns.Add(column);
            return column;
        }

        protected TableIndexConfigurator<TRow, TSearch> ConfigureIndex()
        {
            return new TableIndexConfigurator<TRow, TSearch>(this, AddIndex);
        }

        private void AddIndex(TableIndex<TRow, TSearch> index)
        {
            _indexes.Add(index);
            // First index overrides the default collection to save memory
            if (!(DefaultIndex is TableIndex<TRow, TSearch>))
                DefaultIndex = index;
        }

        public IEnumerable<TRow> Search(TSearch criteria, TableRowCollection<TRow, TSearch> index = null)
        {
            // If no index was provided, we try to find the best one
            if (index == null)
                index = Indexes.OrderByDescending(x => x.GetScore(criteria)).First();

            // If no index was built, we will use the default rows collection
            return (index ?? DefaultIndex).Search(criteria);
        }
    }

}