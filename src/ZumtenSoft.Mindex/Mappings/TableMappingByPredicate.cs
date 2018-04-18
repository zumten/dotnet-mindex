using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.MappingCriterias;

namespace ZumtenSoft.Mindex.Mappings
{
    [DebuggerDisplay(@"\{TableMappingByPredicate " + nameof(Name) + @"={" + nameof(Name) + @"}\}")]
    public class TableMappingByPredicate<TRow, TSearch, TColumn> : ITableMapping<TRow, TSearch>
    {
        public string Name => SearchProperty.Name;
        private readonly Func<TSearch, SearchCriteriaByValue<TColumn>> _getCriteriaValue;
        public Expression<Func<TRow, TColumn, bool>> Predicate { get; }
        public bool IsUnion { get; }

        public MemberInfo SearchProperty { get; }

        public TableMappingByPredicate(Expression<Func<TSearch, SearchCriteriaByValue<TColumn>>> getCriteriaValue, Expression<Func<TRow, TColumn, bool>> predicate, bool isUnion)
        {
            _getCriteriaValue = getCriteriaValue.Compile();
            Predicate = predicate;
            IsUnion = isUnion;
            SearchProperty = ((MemberExpression)getCriteriaValue.Body).Member;
        }

        public TableCriteriaScore GetScore(TSearch search)
        {
            return TableCriteriaScore.NotOptimizable;
        }

        public IEnumerable<TRow> Sort(IEnumerable<TRow> items)
        {
            throw new NotSupportedException($"Mapping with multiple values '{SearchProperty.Name}' does not support indexing");
        }

        public ITableCriteriaForMapping<TRow, TSearch> ExtractCriteria(TSearch search)
        {
            var criteria = _getCriteriaValue(search);
            if (criteria == null)
                return null;

            return new TableCriteriaForMappingByPredicate<TRow,TSearch,TColumn>(this, criteria);
        }

        public bool Reduce(TSearch search, ref BinarySearchTable<TRow> items)
        {
            // Multi-values mapping cannot be searched through indexes, therefore cannot
            // reduce the set of results.
            return false;
        }
    }

}