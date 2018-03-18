using System.Diagnostics;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.ColumnCriterias
{
    [DebuggerDisplay(@"\{TableColumnCriteria Column={Column.Name}, Score={Score.Value}, Criteria={_criteria.Name}\}")]
    public class TableColumnCriteria<TRow, TSearch, TColumn> : ITableColumnCriteria<TRow, TSearch>
    {
        private readonly TableColumn<TRow, TSearch, TColumn> _column;
        private readonly SearchCriteria<TColumn> _criteria;
        public ITableColumn<TRow, TSearch> Column => _column;
        public TableColumnScore Score { get; }

        public TableColumnCriteria(TableColumn<TRow, TSearch, TColumn> column, SearchCriteria<TColumn> criteria)
        {
            _column = column;
            _criteria = criteria;
            Score = GetScore();
        }

        private TableColumnScore GetScore()
        {
            // There are no values, will always return an empty set
            if (_column.PossibleValues.Length == 0)
                return TableColumnScore.Impossible;

            // No criteria or criteria contains no value, we ignore the criteria
            if (_criteria == null)
                return TableColumnScore.NotOptimizable;

            return _criteria.GetScore(_column.PossibleValues, _column.Comparer);
        }

        public BinarySearchResult<TRow> Reduce(BinarySearchResult<TRow> items)
        {
            return _criteria.Reduce(items, _column.GetColumnValue, _column.Comparer);
        }

        public Expression BuildCondition(ParameterExpression paramExpr)
        {
            return _criteria.BuildPredicateExpression(paramExpr, _column.GetColumnExpression, _column.Comparer);
        }
    }
}