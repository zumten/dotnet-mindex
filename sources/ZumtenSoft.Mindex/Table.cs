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
        public List<ITableColumn<TRow, TSearch>> Columns { get; }
        public List<TableIndex<TRow, TSearch>> Indexes { get; }

        public Table()
        {
            Columns = new List<ITableColumn<TRow, TSearch>>();
            Indexes = new List<TableIndex<TRow, TSearch>>();
        }

        protected TableColumn<TRow, TSearch, TColumn> MapSearchCriteria<TColumn>(
            Func<TSearch, SearchCriteria<TColumn>> getSearchValue, Expression<Func<TRow, TColumn>> getColumnValue,
            IComparer<TColumn> comparer = null)
        {
            var column = new TableColumn<TRow, TSearch, TColumn>(getColumnValue, getSearchValue, comparer ?? Comparer<TColumn>.Default);
            Columns.Add(column);
            return column;
        }

        protected void MapMultiValuesSearchCriteria<TColumn>(Func<TSearch, SearchCriteriaByValue<TColumn>> getColumnCriteria, Expression<Func<TRow, TColumn, bool>> predicate)
        {
            Columns.Add(new TableMultiValuesColumn<TRow, TSearch, TColumn>(getColumnCriteria, predicate));
        }

        protected TableIndex<TRow, TSearch> BuildIndex(IReadOnlyCollection<TRow> items, params ITableColumn<TRow, TSearch>[] tableColumns)
        {
            var index = new TableIndex<TRow, TSearch>(this, items, tableColumns);
            Indexes.Add(index);
            return index;
        }

        public IEnumerable<TRow> Search(TSearch criteria, TableIndex<TRow, TSearch> index = null)
        {
            if (index == null)
                index = Indexes.OrderByDescending(x => x.GetScore(criteria)).First();
            return index.Search(criteria);
        }
    }

}