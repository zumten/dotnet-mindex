using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex
{
    public class TableIndexConfigurator<TRow, TSearch>
    {
        private readonly Table<TRow, TSearch> _table;
        private readonly Action<TableIndex<TRow, TSearch>> _addIndex;
        private readonly List<ITableColumn<TRow, TSearch>> _sortColumns;

        public TableIndexConfigurator(Table<TRow, TSearch> table, Action<TableIndex<TRow, TSearch>> addIndex)
        {
            _table = table;
            _addIndex = addIndex;
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
                throw new ArgumentOutOfRangeException("Expression should point directly to a property");

            var column = _table.Columns.FirstOrDefault(x => x.SearchProperty == memberExpr.Member);
            if (column == null)
                throw new ArgumentOutOfRangeException($"Column {memberExpr.Member.Name} has not been mapped");
            return column;
        }

        public TableIndex<TRow, TSearch> Build()
        {
            TableIndex<TRow, TSearch> index = new TableIndex<TRow, TSearch>(_table.DefaultIndex.Rows, _table.Columns, _sortColumns.ToArray());
            _addIndex(index);
            return index;
        }
    }
}