using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Columns
{
    public class TableColumn<TRow, TSearch, TColumn> : ITableColumn<TRow, TSearch>
    {
        public Func<TRow, TColumn> GetColumnValue { get; }
        public Expression<Func<TRow, TColumn>> GetColumnExpression { get; }
        public IComparer<TColumn> Comparer { get; }
        public IEqualityComparer<TColumn> EqualityComparer { get; }
        public TColumn[] PossibleValues { get; }
        public MemberInfo SearchProperty { get; }

        private readonly Func<TSearch, SearchCriteria<TColumn>> _getCriteriaValue;

        public TableColumn(IReadOnlyCollection<TRow> rows, Expression<Func<TRow, TColumn>> getColumnValue, Expression<Func<TSearch, SearchCriteria<TColumn>>> getCriteriaValue, IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer)
        {
            SearchProperty = ((MemberExpression)getCriteriaValue.Body).Member;
            GetColumnValue = getColumnValue.Compile();
            GetColumnExpression = getColumnValue;
            _getCriteriaValue = getCriteriaValue.Compile();
            Comparer = comparer;
            EqualityComparer = equalityComparer;

            var columnValues = new HashSet<TColumn>(rows.Select(GetColumnValue), equalityComparer);
            PossibleValues = columnValues.OrderBy(x => x, comparer).ToArray();
        }

        public IEnumerable<TRow> Sort(IEnumerable<TRow> items)
        {
            return items is IOrderedEnumerable<TRow> orderedItems ? orderedItems.ThenBy(GetColumnValue, Comparer) : items.OrderBy(GetColumnValue, Comparer);
        }

        public ITableColumnCriteria<TRow, TSearch> ExtractCriteria(TSearch search)
        {
            var criteria = _getCriteriaValue(search);
            if (criteria == null || (criteria = criteria.Optimize(Comparer, EqualityComparer)) == null)
                return null;

            return new TableColumnCriteria<TRow,TSearch,TColumn>(this, criteria);
        }
    }

}