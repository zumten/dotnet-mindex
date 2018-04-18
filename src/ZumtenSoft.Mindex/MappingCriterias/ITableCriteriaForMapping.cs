using System.Linq.Expressions;
using ZumtenSoft.Mindex.Mappings;

namespace ZumtenSoft.Mindex.MappingCriterias
{
    public interface ITableCriteriaForMapping<TRow, in TSearch>
    {
        ITableMapping<TRow, TSearch> Mapping { get; }
        TableCriteriaScore Score { get; }
        BinarySearchTable<TRow> Reduce(BinarySearchTable<TRow> items);
        Expression BuildCondition(ParameterExpression paramExprd);
    }
}