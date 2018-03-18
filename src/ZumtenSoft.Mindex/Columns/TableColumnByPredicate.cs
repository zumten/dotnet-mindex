using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Columns
{
    [DebuggerDisplay(@"\{TableColumnByPredicate " + nameof(Name) + @"={" + nameof(Name) + @"}\}")]
    public class TableColumnByPredicate<TRow, TSearch, TColumn> : ITableColumn<TRow, TSearch>
    {
        public string Name => SearchProperty.Name;
        private readonly Func<TSearch, SearchCriteriaByValue<TColumn>> _getCriteriaValue;
        public Expression<Func<TRow, TColumn, bool>> Predicate { get; }
        public bool IsUnion { get; }

        public MemberInfo SearchProperty { get; }

        public TableColumnByPredicate(Expression<Func<TSearch, SearchCriteriaByValue<TColumn>>> getCriteriaValue, Expression<Func<TRow, TColumn, bool>> predicate, bool isUnion)
        {
            _getCriteriaValue = getCriteriaValue.Compile();
            Predicate = predicate;
            IsUnion = isUnion;
            SearchProperty = ((MemberExpression)getCriteriaValue.Body).Member;
        }

        public TableColumnScore GetScore(TSearch search)
        {
            return new TableColumnScore(1, false);
        }

        public IEnumerable<TRow> Sort(IEnumerable<TRow> items)
        {
            throw new NotSupportedException($"Column with multiple values '{SearchProperty.Name}' does not support indexing");
        }

        public ITableCriteriaForColumn<TRow, TSearch> ExtractCriteria(TSearch search)
        {
            var criteria = _getCriteriaValue(search);
            if (criteria == null)
                return null;

            return new TableCriteriaForColumnByPredicate<TRow,TSearch,TColumn>(this, criteria);
        }

        public bool Reduce(TSearch search, ref BinarySearchResult<TRow> items)
        {
            // Multi-values column cannot be searched through indexes, therefore cannot
            // reduce the set of results.
            return false;
        }
    }

}