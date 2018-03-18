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

        public override BinarySearchResult<TRow> Search<TRow>(BinarySearchResult<TRow> rows, Func<TRow, TColumn> getValue, IComparer<TColumn> comparer)
        {
            return null;
        }

        public override Expression BuildPredicateExpression<TRow>(ParameterExpression paramRow, Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer)
        {
            return Expression.Invoke(_predicate,
                Expression.Invoke(getColumnValue, paramRow));
        }

        public override TableColumnScore GetScore(TColumn[] possibleValues, int numberRows, IComparer<TColumn> comparer)
        {
            return new TableColumnScore(1, false);
        }
    }


}