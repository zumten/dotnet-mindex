using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ZumtenSoft.Mindex.Columns
{
    public interface ITableColumn<TRow, in TSearch>
    {
        Tuple<float, bool> GetScore(TSearch search);
        IEnumerable<TRow> Sort(IEnumerable<TRow> items);
        bool Reduce(TSearch search, ref BinarySearchResult<TRow> items);
        Expression BuildCondition(ParameterExpression paramExpr, TSearch criteria);
    }

}