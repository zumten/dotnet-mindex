using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Columns
{
    [DebuggerDisplay(@"\{TableColumnByValue " + nameof(Name) + @"={" + nameof(Name) + @"}\}")]
    public class TableColumnByValue<TRow, TSearch, TColumn> : ITableColumn<TRow, TSearch>
    {
        public string Name => SearchProperty.Name;

        public MemberInfo SearchProperty { get; }
        public TableColumnMetaData<TRow, TColumn> MetaData { get; }

        private readonly Func<TSearch, SearchCriteria<TColumn>> _getCriteriaValue;

        public TableColumnByValue(IReadOnlyCollection<TRow> rows, Expression<Func<TRow, TColumn>> getColumnExpression,
            Expression<Func<TSearch, SearchCriteria<TColumn>>> getCriteriaValue, IComparer<TColumn> comparer,
            IEqualityComparer<TColumn> equalityComparer)
        {
            SearchProperty = ((MemberExpression) getCriteriaValue.Body).Member;
            _getCriteriaValue = getCriteriaValue.Compile();
            MetaData = new TableColumnMetaData<TRow, TColumn>(rows, comparer, equalityComparer, getColumnExpression);
        }

        public IEnumerable<TRow> Sort(IEnumerable<TRow> items)
        {
            return items is IOrderedEnumerable<TRow> orderedItems
                ? orderedItems.ThenBy(MetaData.GetColumnValue, MetaData.Comparer)
                : items.OrderBy(MetaData.GetColumnValue, MetaData.Comparer);
        }

        public ITableCriteriaForColumn<TRow, TSearch> ExtractCriteria(TSearch search)
        {
            var criteria = _getCriteriaValue(search);
            if (criteria == null || (criteria = criteria.Optimize(MetaData)) == null)
                return null;

            return new TableCriteriaForColumnByValue<TRow, TSearch, TColumn>(this, criteria);
        }
    }
}