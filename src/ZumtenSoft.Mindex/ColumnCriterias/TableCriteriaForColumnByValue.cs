using System.Diagnostics;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.ColumnCriterias
{
    [DebuggerDisplay(@"\{TableCriteriaForColumnByValue Column={Column.Name}, Score={Score.Value}, Criteria={_criteria.Name}\}")]
    public class TableCriteriaForColumnByValue<TRow, TSearch, TColumn> : ITableCriteriaForColumn<TRow, TSearch>
    {
        private readonly TableColumnByValue<TRow, TSearch, TColumn> _column;
        private readonly SearchCriteria<TColumn> _criteria;
        private readonly TableColumnMetaData<TRow, TColumn> _metaData;
        public ITableColumn<TRow, TSearch> Column => _column;
        public TableColumnScore Score { get; }

        public TableCriteriaForColumnByValue(TableColumnByValue<TRow, TSearch, TColumn> column, SearchCriteria<TColumn> criteria)
        {
            _column = column;
            _metaData = _column.MetaData;
            _criteria = criteria;
            Score = GetScore(_metaData, criteria);
        }

        public BinarySearchResult<TRow> Reduce(BinarySearchResult<TRow> items)
        {
            return _criteria.Reduce(items, _metaData);
        }

        public Expression BuildCondition(ParameterExpression paramExpr)
        {
            return _criteria.BuildPredicateExpression(paramExpr, _metaData.GetColumnExpression, _metaData.Comparer);
        }

        private static TableColumnScore GetScore(TableColumnMetaData<TRow, TColumn> metaData, SearchCriteria<TColumn> criteria)
        {
            // There are no values, will always return an empty set
            if (metaData.PossibleValues.Length == 0)
                return TableColumnScore.Impossible;

            // No criteria or criteria contains no value, we ignore the criteria
            if (criteria == null)
                return TableColumnScore.NotOptimizable;

            return criteria.GetScore(metaData);
        }
    }
}