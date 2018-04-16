using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.ColumnCriterias
{
    public interface ITableCriteriaForColumn<TRow, in TSearch>
    {
        ITableColumn<TRow, TSearch> Column { get; }
        TableCriteriaScore Score { get; }
        ArraySegmentCollection<TRow> Reduce(ArraySegmentCollection<TRow> items);
        Expression BuildCondition(ParameterExpression paramExprd);
    }
}