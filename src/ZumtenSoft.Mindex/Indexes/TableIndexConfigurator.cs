using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Mappings;

namespace ZumtenSoft.Mindex.Indexes
{
    public class TableIndexConfigurator<TRow, TSearch>
    {
        private readonly TableIndexCollection<TRow, TSearch> _indexes;
        private readonly IReadOnlyCollection<ITableMapping<TRow, TSearch>> _columns;
        private readonly List<ITableMapping<TRow, TSearch>> _sortColumns;

        public TableIndexConfigurator(TableIndexCollection<TRow, TSearch> indexes, IReadOnlyCollection<ITableMapping<TRow, TSearch>> columns)
        {
            _indexes = indexes;
            _columns = columns;
            _sortColumns = new List<ITableMapping<TRow, TSearch>>();
        }

        public TableIndexConfigurator<TRow, TSearch> IncludeColumns(params Expression<Func<TSearch, object>>[] searchCriterias)
        {
            _sortColumns.AddRange(searchCriterias.Select(SearchColumn));
            return this;
        }

        public TableIndexConfigurator<TRow, TSearch> IncludeColumns(params ITableMapping<TRow, TSearch>[] tableMappings)
        {
            _sortColumns.AddRange(tableMappings);
            return this;
        }

        private ITableMapping<TRow, TSearch> SearchColumn(Expression<Func<TSearch, object>> expr)
        {
            if (!(expr.Body is MemberExpression memberExpr))
                throw new ArgumentOutOfRangeException(nameof(expr), "Expression should point directly to a property");

            var column = _columns.FirstOrDefault(x => x.SearchProperty == memberExpr.Member);
            if (column == null)
                throw new ArgumentOutOfRangeException(nameof(expr), $"Mapping {memberExpr.Member.Name} has not been mapped");
            return column;
        }

        public TableIndex<TRow, TSearch> Build()
        {
            TableIndex<TRow, TSearch> index = new TableIndex<TRow, TSearch>(_indexes.DefaultIndex.Rows, _sortColumns.ToArray());
            _indexes.Add(index);
            return index;
        }
    }
}