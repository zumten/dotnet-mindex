using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Indexes;

namespace ZumtenSoft.Mindex.Mappings
{
    public class TableMappingConfigurator<TRow, TSearch, TColumn, TCriteria> where TCriteria : SearchCriteria<TColumn>
    {
        private readonly TRow[] _rows;
        protected readonly TableMappingCollection<TRow, TSearch> Mappings;
        protected readonly Expression<Func<TSearch, TCriteria>> GetCriteriaValue;

        public TableMappingConfigurator(TableMappingCollection<TRow, TSearch> mappings, TRow[] rows, Expression<Func<TSearch, TCriteria>> getCriteriaValue)
        {
            Mappings = mappings;
            _rows = rows;
            GetCriteriaValue = getCriteriaValue;
        }

        public TableMappingByValue<TRow, TSearch, TColumn, TCriteria> ToProperty(Expression<Func<TRow, TColumn>> getColumnValue, IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer)
        {
            var column = new TableMappingByValue<TRow, TSearch, TColumn, TCriteria>(_rows, getColumnValue, GetCriteriaValue, comparer, equalityComparer);
            Mappings.Add(column);
            return column;
        }

        public TableMappingByValue<TRow, TSearch, TColumn, TCriteria> ToProperty<TComparer>(Expression<Func<TRow, TColumn>> getColumnValue, TComparer hybridComparer) where TComparer : IComparer<TColumn>, IEqualityComparer<TColumn>
        {
            return ToProperty(getColumnValue, hybridComparer, hybridComparer);
        }

        public TableMappingByValue<TRow, TSearch, TColumn, TCriteria> ToProperty(Expression<Func<TRow, TColumn>> getColumnValue)
        {
            return ToProperty(getColumnValue, Comparer<TColumn>.Default, EqualityComparer<TColumn>.Default);
        }
    }

    public class TableMappingByValueConfigurator<TRow, TSearch, TColumn> : TableMappingConfigurator<TRow, TSearch, TColumn, SearchCriteriaByValue<TColumn>>
    {
        public TableMappingByValueConfigurator(TableMappingCollection<TRow, TSearch> mappings, TRow[] rows, Expression<Func<TSearch, SearchCriteriaByValue<TColumn>>> getCriteriaValue)
            : base(mappings, rows, getCriteriaValue)
        {
        }

        public TableMappingByPredicate<TRow, TSearch, TColumn> ToPredicate(Expression<Func<TRow, TColumn, bool>> columnPredicate, bool isUnion = false)
        {
            var column = new TableMappingByPredicate<TRow, TSearch, TColumn>(GetCriteriaValue, columnPredicate, isUnion);
            Mappings.Add(column);
            return column;
        }
    }
}