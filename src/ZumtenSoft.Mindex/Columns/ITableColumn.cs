using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Reflection;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Columns
{
    public interface ITableColumn<TRow, in TSearch>
    {
        MemberInfo SearchProperty { get; }
        IEnumerable<TRow> Sort(IEnumerable<TRow> items);
        ITableColumnCriteria<TRow, TSearch> ExtractCriteria(TSearch search);
    }
}