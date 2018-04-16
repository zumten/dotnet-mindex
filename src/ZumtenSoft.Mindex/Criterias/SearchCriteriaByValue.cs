using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.ColumnCriterias;
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

        public override string Name => String.Join(", ", SearchValues);

        public override ArraySegmentCollection<TRow> Reduce<TRow>(ArraySegmentCollection<TRow> rows, TableColumnMetaData<TRow, TColumn> metaData)
        {
            return rows.ReduceByValues(metaData.GetColumnValue, SearchValues, metaData.Comparer);
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

            var compareMethod = typeof(IComparer<TColumn>).GetMethod("Compare")
                ?? throw new InvalidOperationException("Type IComparer should implement the method Compare");

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

        public override SearchCriteria<TColumn> Optimize<TRow>(TableColumnMetaData<TRow, TColumn> metaData)
        {
            switch (SearchValues.Length)
            {
                case 0: return null;
                case 1: return this;
            }

            // Try to remove duplicate search values
            return ByValues(SearchValues.Distinct(metaData.EqualityComparer).ToArray());
        }

        public override TableCriteriaScore GetScore<TRow>(TableColumnMetaData<TRow, TColumn> metaData)
        {
            if (SearchValues.Length == 0)
                return new TableCriteriaScore(1, false);
            return new TableCriteriaScore(SearchValues.Length / (float)metaData.PossibleValues.Length, true);
        }
    }


}