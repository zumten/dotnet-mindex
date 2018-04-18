using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Mappings;

namespace ZumtenSoft.Mindex.MappingCriterias
{
    [DebuggerDisplay(@"\{TableCriteriaForColumnByPredicate Mapping={Mapping.Name}, Score={Score.Value}, Criteria={_criteria.Name}\}")]
    public class TableCriteriaForMappingByPredicate<TRow, TSearch, TColumn> : ITableCriteriaForMapping<TRow, TSearch>
    {
        private readonly TableMappingByPredicate<TRow, TSearch, TColumn> _mapping;
        private readonly SearchCriteriaByValue<TColumn> _criteria;
        public ITableMapping<TRow, TSearch> Mapping => _mapping;

        public TableCriteriaForMappingByPredicate(TableMappingByPredicate<TRow, TSearch, TColumn> mapping, SearchCriteriaByValue<TColumn> criteria)
        {
            _mapping = mapping;
            _criteria = criteria;
        }

        public TableCriteriaScore Score => TableCriteriaScore.NotOptimizable;
        public BinarySearchTable<TRow> Reduce(BinarySearchTable<TRow> items)
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
                        _mapping.Predicate,
                        paramExpr,
                        Expression.Constant(value)));
            }

            return _mapping.IsUnion
                ? expressions.Aggregate(Expression.OrElse)
                : expressions.Aggregate(Expression.AndAlso);
        }
    }
}