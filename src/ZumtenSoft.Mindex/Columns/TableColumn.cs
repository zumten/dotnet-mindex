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
        private readonly Func<TRow, TColumn> _getColumnValue;
        private readonly Expression<Func<TRow, TColumn>> _getColumnExpression;
        private readonly Func<TSearch, SearchCriteria<TColumn>> _getCriteriaValue;
        private readonly IComparer<TColumn> _comparer;

        private readonly int _numberRows;
        private readonly TColumn[] _possibleValues;

        public MemberInfo SearchProperty { get; }

        public TableColumn(IReadOnlyCollection<TRow> rows, Expression<Func<TRow, TColumn>> getColumnValue, Expression<Func<TSearch, SearchCriteria<TColumn>>> getCriteriaValue, IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer)
        {
            SearchProperty = ((MemberExpression)getCriteriaValue.Body).Member;
            _getColumnValue = getColumnValue.Compile();
            _getColumnExpression = getColumnValue;
            _getCriteriaValue = getCriteriaValue.Compile();
            _comparer = comparer;

            var columnValues = new HashSet<TColumn>(rows.Select(_getColumnValue), equalityComparer);
            _numberRows = rows.Count;
            _possibleValues = columnValues.OrderBy(x => x, comparer).ToArray();
        }

        public TableColumnScore GetScore(TSearch search)
        {
            var criteria = _getCriteriaValue(search);
            var criteriaByValue = criteria as SearchCriteriaByValue<TColumn>;

            // There are no values, will always return an empty set
            if (_possibleValues.Length == 0)
                return new TableColumnScore(0, false);

            // No criteria or criteria contains no value, we ignore the criteria
            if (criteria == null)
                return new TableColumnScore(1, false);

            return criteria.GetScore(_possibleValues, _numberRows, _comparer);
        }

        public IEnumerable<TRow> Sort(IEnumerable<TRow> items)
        {
            return items is IOrderedEnumerable<TRow> orderedItems ? orderedItems.ThenBy(_getColumnValue, _comparer) : items.OrderBy(_getColumnValue, _comparer);
        }

        public bool Reduce(TSearch search, ref BinarySearchResult<TRow> items)
        {
            var criteria = _getCriteriaValue(search);
            if (criteria != null)
            {
                items = criteria.Search(items, _getColumnValue, _comparer);
                return true;
            }
            return false;
        }

        public Expression BuildCondition(ParameterExpression paramExpr, TSearch search)
        {
            var criteria = _getCriteriaValue(search);
            return criteria?.BuildPredicateExpression(paramExpr, _getColumnExpression, _comparer);
        }
    }

}