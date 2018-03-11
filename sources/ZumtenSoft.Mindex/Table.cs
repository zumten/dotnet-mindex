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
        public IReadOnlyCollection<TRow> Rows { get; set; }
        public List<ITableColumn<TRow, TSearch>> Columns { get; }
        public List<TableIndex<TRow, TSearch>> Indexes { get; }

        protected Table(IReadOnlyCollection<TRow> rows)
        {
            Rows = rows;
            Columns = new List<ITableColumn<TRow, TSearch>>();
            Indexes = new List<TableIndex<TRow, TSearch>>();
        }

        protected TableColumn<TRow, TSearch, TColumn> MapSearchCriteria<TColumn>(
            Expression<Func<TSearch, SearchCriteria<TColumn>>> getSearchValue,
            Expression<Func<TRow, TColumn>> getColumnValue,
            IComparer<TColumn> comparer = null)
        {
            var column = new TableColumn<TRow, TSearch, TColumn>(getColumnValue, getSearchValue, comparer ?? Comparer<TColumn>.Default);
            Columns.Add(column);
            return column;
        }

        protected void MapMultiValuesSearchCriteria<TColumn>(Expression<Func<TSearch, SearchCriteriaByValue<TColumn>>> getColumnCriteria, Expression<Func<TRow, TColumn, bool>> predicate)
        {
            Columns.Add(new TableMultiValuesColumn<TRow, TSearch, TColumn>(getColumnCriteria, predicate));
        }

        protected TableIndex<TRow, TSearch> BuildIndex(params Expression<Func<TSearch, object>>[] searchCriterias)
        {
            return BuildIndex(searchCriterias.Select(SearchColumn).ToArray());
        }

        protected TableIndex<TRow, TSearch> BuildIndex(params ITableColumn<TRow, TSearch>[] tableColumns)
        {
            var index = new TableIndex<TRow, TSearch>(this, Rows, tableColumns);
            Indexes.Add(index);
            return index;
        }

        private ITableColumn<TRow, TSearch> SearchColumn(Expression<Func<TSearch, object>> expr)
        {
            if (!(expr.Body is MemberExpression memberExpr))
                throw new ArgumentOutOfRangeException("Expression should point directly to a property");

            var column = Columns.FirstOrDefault(x => x.SearchProperty == memberExpr.Member);
            if (column == null)
                throw new ArgumentOutOfRangeException($"Column {memberExpr.Member.Name} has not been mapped");
            return column;
        }

        public IEnumerable<TRow> Search(TSearch criteria, TableIndex<TRow, TSearch> index = null)
        {
            if (index == null)
                index = Indexes.OrderByDescending(x => x.GetScore(criteria)).First();
            return index.Search(criteria);
        }
    }

}