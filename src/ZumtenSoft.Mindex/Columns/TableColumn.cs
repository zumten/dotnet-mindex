using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Columns
{
    public class TableColumn<TRow, TSearch, TColumn> : ITableColumn<TRow, TSearch>
    {
        public Func<TRow, TColumn> GetColumnValue { get; }
        public Expression<Func<TRow, TColumn>> GetColumnExpression { get; }
        public Func<TSearch, SearchCriteria<TColumn>> GetCriteriaValue { get; }
        public IComparer<TColumn> Comparer { get; }
        public MemberInfo SearchProperty { get; }

        public TableColumn(Expression<Func<TRow, TColumn>> getColumnValue, Expression<Func<TSearch, SearchCriteria<TColumn>>> getCriteriaValue, IComparer<TColumn> comparer)
        {
            SearchProperty = ((MemberExpression)getCriteriaValue.Body).Member;
            GetColumnValue = getColumnValue.Compile();
            GetColumnExpression = getColumnValue;
            GetCriteriaValue = getCriteriaValue.Compile();
            Comparer = comparer;
        }

        public Tuple<float, bool> GetScore(TSearch search)
        {
            var criteria = GetCriteriaValue(search);
            return criteria is SearchCriteriaByRange<TColumn> ? Tuple.Create(1.5f, false)
                : criteria is SearchCriteriaByValue<TColumn> ? Tuple.Create(1f, true)
                : Tuple.Create(0f, false);
        }

        public IEnumerable<TRow> Sort(IEnumerable<TRow> items)
        {
            return items is IOrderedEnumerable<TRow> orderedItems ? orderedItems.ThenBy(GetColumnValue, Comparer) : items.OrderBy(GetColumnValue, Comparer);
        }

        public bool Reduce(TSearch search, ref BinarySearchResult<TRow> items)
        {
            var criteria = GetCriteriaValue(search);
            if (criteria != null)
            {
                items = criteria.Search(items, GetColumnValue, Comparer);
                return true;
            }
            return false;
        }

        public Expression BuildCondition(ParameterExpression paramExpr, TSearch search)
        {
            var criteria = GetCriteriaValue(search);
            return criteria?.BuildPredicateExpression(paramExpr, GetColumnExpression, Comparer);
        }
    }

}