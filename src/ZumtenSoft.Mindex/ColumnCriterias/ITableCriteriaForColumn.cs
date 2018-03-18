using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.ColumnCriterias
{
    public interface ITableCriteriaForColumn<TRow, in TSearch>
    {
        ITableColumn<TRow, TSearch> Column { get; }
        TableColumnScore Score { get; }
        BinarySearchResult<TRow> Reduce(BinarySearchResult<TRow> items);
        Expression BuildCondition(ParameterExpression paramExprd);
    }
}