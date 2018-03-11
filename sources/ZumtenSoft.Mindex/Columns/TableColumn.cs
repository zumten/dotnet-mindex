using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Columns
{
    public class TableColumn<TTable, TSearch, TColumn> : ITableColumn<TTable, TSearch>
    {
        public Func<TTable, TColumn> GetColumnValue { get; }
        public Expression<Func<TTable, TColumn>> GetColumnExpression { get; }
        public Func<TSearch, SearchCriteria<TColumn>> GetCriteriaValue { get; }
        public IComparer<TColumn> Comparer { get; }

        public TableColumn(Expression<Func<TTable, TColumn>> getColumnValue, Func<TSearch, SearchCriteria<TColumn>> getCriteriaValue, IComparer<TColumn> comparer)
        {
            GetColumnValue = getColumnValue.Compile();
            GetColumnExpression = getColumnValue;
            GetCriteriaValue = getCriteriaValue;
            Comparer = comparer;
        }

        public Tuple<float, bool> GetScore(TSearch search)
        {
            var criteria = GetCriteriaValue(search);
            return criteria is SearchCriteriaByRange<TColumn> ? Tuple.Create(1.5f, false)
                : criteria is SearchCriteriaByValue<TColumn> ? Tuple.Create(1f, true)
                : Tuple.Create(0f, false);
        }

        public IEnumerable<TTable> Sort(IEnumerable<TTable> items)
        {
            return items is IOrderedEnumerable<TTable> orderedItems ? orderedItems.ThenBy(GetColumnValue, Comparer) : items.OrderBy(GetColumnValue, Comparer);
        }

        public bool Reduce(TSearch search, ref BinarySearchResult<TTable> items)
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