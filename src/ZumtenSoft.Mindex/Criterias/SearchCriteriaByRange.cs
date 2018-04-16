using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Criterias
{
    public class SearchCriteriaByRange<TColumn> : SearchCriteria<TColumn>
    {
        public TColumn Start { get; }
        public TColumn End { get; }
        public bool PreserveSearchability { get; }

        public SearchCriteriaByRange(TColumn start, TColumn end, bool preserveSearchability)
        {
            Start = start;
            End = end;
            PreserveSearchability = preserveSearchability;
        }

        public override string Name => Start + "-" + End;

        public override ArraySegmentCollection<TRow> Reduce<TRow>(ArraySegmentCollection<TRow> rows, TableColumnMetaData<TRow, TColumn> metaData)
        {
            return rows.ReduceByRange(metaData.GetColumnValue, Start, End, metaData.Comparer, PreserveSearchability);
        }

        public override Expression BuildPredicateExpression<TRow>(ParameterExpression paramRow, Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer)
        {
            var compareMethod = typeof(IComparer<TColumn>).GetMethod("Compare")
                ?? throw new InvalidOperationException("Type IComparer should implement the method Compare");

            var exprVariable = Expression.Variable(typeof(TColumn), "columnValue");
            return Expression.Block(
                new[] { exprVariable },
                Expression.Assign(exprVariable,
                    Expression.Invoke(getColumnValue, paramRow)),
                Expression.AndAlso(
                    Expression.GreaterThanOrEqual(
                        Expression.Call(Expression.Constant(comparer), compareMethod, exprVariable, Expression.Constant(Start)),
                        Expression.Constant(0)),
                    Expression.LessThanOrEqual(
                        Expression.Call(Expression.Constant(comparer), compareMethod, exprVariable, Expression.Constant(End)),
                        Expression.Constant(0))));
        }

        public override SearchCriteria<TColumn> Optimize<TRow>(TableColumnMetaData<TRow, TColumn> metaData)
        {
            if (metaData.Comparer.Compare(Start, End) == 0)
                return ByValues(Start);
            var resultRange = new ArraySegmentCollection<TColumn>(metaData.PossibleValues).ReduceByRange(x => x, Start, End, metaData.Comparer);
            var nbResults = resultRange.TotalCount;
            if (nbResults <= 1)
                return ByValues(resultRange.Materialize());
            if (nbResults < 10)
                return new SearchCriteriaByRange<TColumn>(Start, End, true);
            return this;
        }

        public override TableCriteriaScore GetScore<TRow>(TableColumnMetaData<TRow, TColumn> metaData)
        {
            var resultRange = new ArraySegmentCollection<TColumn>(metaData.PossibleValues).ReduceByRange(x => x, Start, End, metaData.Comparer);
            var nbResults = resultRange.TotalCount;
            if (nbResults == 0)
                return TableCriteriaScore.Impossible;
            return new TableCriteriaScore((float)nbResults / metaData.PossibleValues.Length, PreserveSearchability);
        }
    }


}