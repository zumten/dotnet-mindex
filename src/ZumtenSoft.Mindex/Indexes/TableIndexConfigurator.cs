using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Indexes
{
    public class TableIndexConfigurator<TRow, TSearch>
    {
        private readonly TableIndexCollection<TRow, TSearch> _indexes;
        private readonly IReadOnlyCollection<ITableColumn<TRow, TSearch>> _columns;
        private readonly List<ITableColumn<TRow, TSearch>> _sortColumns;

        public TableIndexConfigurator(TableIndexCollection<TRow, TSearch> indexes, IReadOnlyCollection<ITableColumn<TRow, TSearch>> columns)
        {
            _indexes = indexes;
            _columns = columns;
            _sortColumns = new List<ITableColumn<TRow, TSearch>>();
        }

        public TableIndexConfigurator<TRow, TSearch> IncludeColumns(params Expression<Func<TSearch, object>>[] searchCriterias)
        {
            _sortColumns.AddRange(searchCriterias.Select(SearchColumn));
            return this;
        }

        public TableIndexConfigurator<TRow, TSearch> IncludeColumns(params ITableColumn<TRow, TSearch>[] tableColumns)
        {
            _sortColumns.AddRange(tableColumns);
            return this;
        }

        private ITableColumn<TRow, TSearch> SearchColumn(Expression<Func<TSearch, object>> expr)
        {
            if (!(expr.Body is MemberExpression memberExpr))
                throw new ArgumentOutOfRangeException(nameof(expr), "Expression should point directly to a property");

            var column = _columns.FirstOrDefault(x => x.SearchProperty == memberExpr.Member);
            if (column == null)
                throw new ArgumentOutOfRangeException(nameof(expr), $"Column {memberExpr.Member.Name} has not been mapped");
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