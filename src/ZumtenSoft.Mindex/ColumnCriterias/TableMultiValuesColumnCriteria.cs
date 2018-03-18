using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.ColumnCriterias
{
    public class TableMultiValuesColumnCriteria<TRow, TSearch, TColumn> : ITableColumnCriteria<TRow, TSearch>
    {
        private readonly TableMultiValuesColumn<TRow, TSearch, TColumn> _column;
        private readonly SearchCriteriaByValue<TColumn> _criteria;
        public ITableColumn<TRow, TSearch> Column => _column;

        public TableMultiValuesColumnCriteria(TableMultiValuesColumn<TRow, TSearch, TColumn> column, SearchCriteriaByValue<TColumn> criteria)
        {
            _column = column;
            _criteria = criteria;
        }

        public TableColumnScore Score => TableColumnScore.NotOptimizable;
        public BinarySearchResult<TRow> Reduce(BinarySearchResult<TRow> items)
        {
            return null;
        }

        public Expression BuildCondition(ParameterExpression paramExpr)
        {
            List<Expression> expressions = new List<Expression>();
            foreach (var value in _criteria.SearchValues)
            {
                expressions.Add(
                    Expression.Invoke(
                        _column.Predicate,
                        paramExpr,
                        Expression.Constant(value)));
            }

            return _column.IsUnion
                ? expressions.Aggregate(Expression.OrElse)
                : expressions.Aggregate(Expression.AndAlso);
        }
    }
}