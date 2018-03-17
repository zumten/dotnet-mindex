using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ZumtenSoft.Mindex.Columns
{
    public interface ITableColumn<TRow, in TSearch>
    {
        MemberInfo SearchProperty { get; }
        TableColumnScore GetScore(TSearch search);
        IEnumerable<TRow> Sort(IEnumerable<TRow> items);
        bool Reduce(TSearch search, ref BinarySearchResult<TRow> items);
        Expression BuildCondition(ParameterExpression paramExpr, TSearch criteria);
    }

}