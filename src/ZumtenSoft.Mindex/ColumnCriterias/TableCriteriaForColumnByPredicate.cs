using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.ColumnCriterias
{
    [DebuggerDisplay(@"\{TableCriteriaForColumnByPredicate Column={Column.Name}, Score={Score.Value}, Criteria={_criteria.Name}\}")]
    public class TableCriteriaForColumnByPredicate<TRow, TSearch, TColumn> : ITableCriteriaForColumn<TRow, TSearch>
    {
        private readonly TableColumnByPredicate<TRow, TSearch, TColumn> _column;
        private readonly SearchCriteriaByValue<TColumn> _criteria;
        public ITableColumn<TRow, TSearch> Column => _column;

        public TableCriteriaForColumnByPredicate(TableColumnByPredicate<TRow, TSearch, TColumn> column, SearchCriteriaByValue<TColumn> criteria)
        {
            _column = column;
            _criteria = criteria;
        }

        public TableCriteriaScore Score => TableCriteriaScore.NotOptimizable;
        public ArraySegmentCollection<TRow> Reduce(ArraySegmentCollection<TRow> items)
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