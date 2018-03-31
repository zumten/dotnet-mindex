using System.Collections.Generic;
using System.Reflection;
using ZumtenSoft.Mindex.ColumnCriterias;

namespace ZumtenSoft.Mindex.Columns
{
    public interface ITableColumn<TRow, in TSearch>
    {
        string Name { get; }
        MemberInfo SearchProperty { get; }
        IEnumerable<TRow> Sort(IEnumerable<TRow> items);
        ITableCriteriaForColumn<TRow, TSearch> ExtractCriteria(TSearch search);
    }
}