using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.ColumnCriterias;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Indexes
{
    public class TableRowCollection<TRow, TSearch>
    {
        public TRow[] Rows { get; }

        public TableRowCollection(TRow[] rows)
        {
            Rows = rows;
        }

        public virtual IEnumerable<TRow> Search(IReadOnlyCollection<ITableCriteriaForColumn<TRow, TSearch>> criterias)
        {
            return FilterRowsWithCustomExpression(Rows, criterias);
        }

        protected static IEnumerable<TRow> FilterRowsWithCustomExpression(IEnumerable<TRow> items, IReadOnlyCollection<ITableCriteriaForColumn<TRow, TSearch>> columns)
        {
            if (columns.Count == 0)
                return items;

            ParameterExpression paramExpr = Expression.Parameter(typeof(TRow), "row");
            IList<Expression> conditions = new List<Expression>();
            foreach (var column in columns)
            {
                var condition = column.BuildCondition(paramExpr);
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