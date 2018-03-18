using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Criterias
{
    public class SearchCriteriaByValue<TColumn> : SearchCriteria<TColumn>
    {
        public TColumn[] SearchValues { get; }

        public SearchCriteriaByValue(TColumn[] searchValues)
        {
            SearchValues = searchValues;
        }

        public override BinarySearchResult<TRow> Reduce<TRow>(BinarySearchResult<TRow> rows, Func<TRow, TColumn> getValue, IComparer<TColumn> comparer)
        {
            return rows.ReduceIn(getValue, SearchValues, comparer);
        }

        public static implicit operator SearchCriteriaByValue<TColumn>(TColumn value)
        {
            return ByValues(value);
        }

        public static implicit operator SearchCriteriaByValue<TColumn>(TColumn[] values)
        {
            return ByValues(values);
        }

        public override Expression BuildPredicateExpression<TRow>(ParameterExpression paramRow, Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer)
        {
            if (SearchValues.Length == 0)
                return Expression.Constant(false);

            var compareMethod = typeof(IComparer<TColumn>).GetMethod("Compare");
            List<Expression> expressions = new List<Expression>();
            foreach (var value in SearchValues)
            {
                expressions.Add(
                    Expression.Equal(
                        Expression.Call(
                            Expression.Constant(comparer),
                            compareMethod,
                            Expression.Invoke(getColumnValue, paramRow),
                            Expression.Constant(value)),
                        Expression.Constant(0)));
            }

            return expressions.Aggregate((x, y) => Expression.OrElse(y, x));
        }

        public override SearchCriteria<TColumn> Optimize(IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer)
        {
            if (SearchValues.Length == 0)
                return null;
            if (SearchValues.Length == 1)
                return this;

            return ByValues(new HashSet<TColumn>(SearchValues, equalityComparer).ToArray());
        }

        public override TableColumnScore GetScore(TColumn[] possibleValues, IComparer<TColumn> comparer)
        {
            if (SearchValues.Length == 0)
                return new TableColumnScore(1, false);
            return new TableColumnScore(SearchValues.Length / (float)possibleValues.Length, true);
        }
    }


}