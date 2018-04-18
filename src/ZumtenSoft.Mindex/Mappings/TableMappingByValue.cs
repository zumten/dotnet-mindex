using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.MappingCriterias;

namespace ZumtenSoft.Mindex.Mappings
{
    [DebuggerDisplay(@"\{TableMappingByValue " + nameof(Name) + @"={" + nameof(Name) + @"}\}")]
    public class TableMappingByValue<TRow, TSearch, TColumn, TCriteria> : ITableMapping<TRow, TSearch> where TCriteria : SearchCriteria<TColumn>
    {
        public string Name => SearchProperty.Name;

        public MemberInfo SearchProperty { get; }
        public TableMappingMetaData<TRow, TColumn> MetaData { get; }

        private readonly Func<TSearch, TCriteria> _getCriteriaValue;

        public TableMappingByValue(IReadOnlyCollection<TRow> rows, Expression<Func<TRow, TColumn>> getColumnExpression,
            Expression<Func<TSearch, TCriteria>> getCriteriaValue, IComparer<TColumn> comparer,
            IEqualityComparer<TColumn> equalityComparer)
        {
            SearchProperty = ((MemberExpression) getCriteriaValue.Body).Member;
            _getCriteriaValue = getCriteriaValue.Compile();
            MetaData = new TableMappingMetaData<TRow, TColumn>(rows, comparer, equalityComparer, getColumnExpression);
        }

        public IEnumerable<TRow> Sort(IEnumerable<TRow> items)
        {
            return items is IOrderedEnumerable<TRow> orderedItems
                ? orderedItems.ThenBy(MetaData.GetTargetValue, MetaData.Comparer)
                : items.OrderBy(MetaData.GetTargetValue, MetaData.Comparer);
        }

        public ITableCriteriaForMapping<TRow, TSearch> ExtractCriteria(TSearch search)
        {
            SearchCriteria<TColumn> criteria = _getCriteriaValue(search);
            if (criteria == null || (criteria = criteria.Optimize(MetaData)) == null)
                return null;

            return new TableCriteriaForMappingByValue<TRow, TSearch, TColumn, TCriteria>(this, criteria);
        }
    }
}