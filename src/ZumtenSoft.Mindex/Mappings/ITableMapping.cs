using System.Collections.Generic;
using System.Reflection;
using ZumtenSoft.Mindex.MappingCriterias;

namespace ZumtenSoft.Mindex.Mappings
{
    public interface ITableMapping<TRow, in TSearch>
    {
        string Name { get; }
        MemberInfo SearchProperty { get; }
        IEnumerable<TRow> Sort(IEnumerable<TRow> items);
        ITableCriteriaForMapping<TRow, TSearch> ExtractCriteria(TSearch search);
    }
}