using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;

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

        public override BinarySearchResult<TRow> Reduce<TRow>(BinarySearchResult<TRow> rows, Func<TRow, TColumn> getValue, IComparer<TColumn> comparer)
        {
            return null;
        }

        public override Expression BuildPredicateExpression<TRow>(ParameterExpression paramRow, Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer)
        {
            return Expression.Invoke(_predicate, getColumnValue);
        }

        public override SearchCriteria<TColumn> Optimize(IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer)
        {
            return this;
        }

        public override TableColumnScore GetScore(TColumn[] possibleValues, IComparer<TColumn> comparer)
        {
            return TableColumnScore.NotOptimizable;
        }
    }


}