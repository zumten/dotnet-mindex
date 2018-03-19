using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Criterias
{
    public class SearchCriteriaByRange<TColumn> : SearchCriteria<TColumn>
    {
        public TColumn Start { get; }
        public TColumn End { get; }

        public SearchCriteriaByRange(TColumn start, TColumn end)
        {
            Start = start;
            End = end;
        }

        public override string Name => Start + "-" + End;

        public override BinarySearchResult<TRow> Reduce<TRow>(BinarySearchResult<TRow> rows, TableColumnMetaData<TRow, TColumn> metaData)
        {
            return rows.ReduceRange(metaData.GetColumnValue, Start, End, metaData.Comparer);
        }

        public override Expression BuildPredicateExpression<TRow>(ParameterExpression paramRow, Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer)
        {
            var compareMethod = typeof(IComparer<TColumn>).GetMethod("Compare");
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
            return this;
        }

        public override TableCriteriaScore GetScore<TRow>(TableColumnMetaData<TRow, TColumn> metaData)
        {
            var resultRange = new BinarySearchResult<TColumn>(metaData.PossibleValues).ReduceRange(x => x, Start, End, metaData.Comparer);
            if (resultRange.Count == 0)
                return TableCriteriaScore.Impossible;
            return new TableCriteriaScore((float)resultRange.Count / metaData.PossibleValues.Length, false);
        }
    }


}