using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.MappingCriterias;
using ZumtenSoft.Mindex.Mappings;

namespace ZumtenSoft.Mindex.Criterias
{
    public class SearchCriteriaByPredicate<TColumn> : SearchCriteria<TColumn>
    {
        private readonly Expression<Func<TColumn, bool>> _predicate;

        public SearchCriteriaByPredicate(Expression<Func<TColumn, bool>> predicate)
        {
            _predicate = predicate;
        }

        public override string Name => _predicate.ToString();

        public override BinarySearchTable<TRow> Reduce<TRow>(BinarySearchTable<TRow> rows, TableMappingMetaData<TRow, TColumn> metaData)
        {
            return null;
        }

        public override Expression BuildPredicateExpression<TRow>(ParameterExpression paramRow, Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer)
        {
            return Expression.Invoke(_predicate,
                Expression.Invoke(getColumnValue, paramRow));
        }

        public override SearchCriteria<TColumn> Optimize<TRow>(TableMappingMetaData<TRow, TColumn> metaData)
        {
            return this;
        }

        public override TableCriteriaScore GetScore<TRow>(TableMappingMetaData<TRow, TColumn> metaData)
        {
            return TableCriteriaScore.NotOptimizable;
        }
    }


}