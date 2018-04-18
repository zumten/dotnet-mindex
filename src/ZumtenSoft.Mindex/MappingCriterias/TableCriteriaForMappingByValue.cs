using System.Diagnostics;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Mappings;

namespace ZumtenSoft.Mindex.MappingCriterias
{
    [DebuggerDisplay(@"\{TableCriteriaForMappingByValue Mapping={Mapping.Name}, Score={Score.Value}, Criteria={_criteria.Name}\}")]
    public class TableCriteriaForMappingByValue<TRow, TSearch, TColumn, TCriteria> : ITableCriteriaForMapping<TRow, TSearch> where TCriteria : SearchCriteria<TColumn>
    {
        private readonly TableMappingByValue<TRow, TSearch, TColumn, TCriteria> _mapping;
        private readonly SearchCriteria<TColumn> _criteria;
        private readonly TableMappingMetaData<TRow, TColumn> _metaData;
        public ITableMapping<TRow, TSearch> Mapping => _mapping;
        public TableCriteriaScore Score { get; }

        public TableCriteriaForMappingByValue(TableMappingByValue<TRow, TSearch, TColumn, TCriteria> mapping, SearchCriteria<TColumn> criteria)
        {
            _mapping = mapping;
            _metaData = _mapping.MetaData;
            _criteria = criteria;
            Score = GetScore(_metaData, criteria);
        }

        public BinarySearchTable<TRow> Reduce(BinarySearchTable<TRow> items)
        {
            return _criteria.Reduce(items, _metaData);
        }

        public Expression BuildCondition(ParameterExpression paramExpr)
        {
            return _criteria.BuildPredicateExpression(paramExpr, _metaData.GetTargetExpression, _metaData.Comparer);
        }

        private static TableCriteriaScore GetScore(TableMappingMetaData<TRow, TColumn> metaData, SearchCriteria<TColumn> criteria)
        {
            // There are no values, will always return an empty set
            if (metaData.PossibleValues.Length == 0)
                return TableCriteriaScore.Impossible;

            // No criteria or criteria contains no value, we ignore the criteria
            if (criteria == null)
                return TableCriteriaScore.NotOptimizable;

            return criteria.GetScore(metaData);
        }
    }
}