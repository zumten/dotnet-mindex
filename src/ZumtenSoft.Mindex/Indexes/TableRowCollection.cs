using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Indexes
{
    public class TableRowCollection<TRow, TSearch>
    {
        public TRow[] Rows { get; }
        protected IReadOnlyCollection<ITableColumn<TRow, TSearch>> Columns { get; }

        public TableRowCollection(TRow[] rows, IReadOnlyCollection<ITableColumn<TRow, TSearch>> columns)
        {
            Rows = rows;
            Columns = columns;
        }

        public virtual IEnumerable<TRow> Search(TSearch search)
        {
            return FilterRowsWithCustomExpression(Rows, search, Columns);
        }

        protected static IEnumerable<TRow> FilterRowsWithCustomExpression(IEnumerable<TRow> items, TSearch search, IReadOnlyCollection<ITableColumn<TRow, TSearch>> columns)
        {
            if (columns.Count == 0)
                return items;

            ParameterExpression paramExpr = Expression.Parameter(typeof(TRow), "row");
            IList<Expression> conditions = new List<Expression>();
            foreach (var column in columns)
            {
                var condition = column.BuildCondition(paramExpr, search);
                if (condition != null)
                    conditions.Add(condition);
            }

            if (conditions.Count == 0)
                return items;

            var joinedConditions = conditions.Reverse().Aggregate((x, y) => Expression.AndAlso(y, x));
            var lambda = Expression.Lambda<Func<TRow, bool>>(joinedConditions, paramExpr);
            return items.Where(lambda.Compile());
        }
    }
}