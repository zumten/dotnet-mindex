using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.ColumnCriterias;

namespace ZumtenSoft.Mindex.Indexes
{
    public class TableRowCollection<TRow, TSearch>
    {
        public TRow[] Rows { get; }

        public TableRowCollection(TRow[] rows)
        {
            Rows = rows;
        }

        public virtual TRow[] Search(IReadOnlyCollection<ITableCriteriaForColumn<TRow, TSearch>> criterias)
        {
            return FilterRowsWithCustomExpression(new BinarySearchResult<TRow>(Rows), criterias);
        }

        protected static TRow[] FilterRowsWithCustomExpression(BinarySearchResult<TRow> items, IReadOnlyCollection<ITableCriteriaForColumn<TRow, TSearch>> columns)
        {
            if (columns.Count == 0)
                return items.Materialize();

            ParameterExpression paramExpr = Expression.Parameter(typeof(TRow), "row");
            IList<Expression> conditions = new List<Expression>();
            foreach (var column in columns)
            {
                var condition = column.BuildCondition(paramExpr);
                if (condition != null)
                    conditions.Add(condition);
            }

            if (conditions.Count == 0)
                return items.Materialize();

            var joinedConditions = conditions.Reverse().Aggregate((x, y) => Expression.AndAlso(y, x));
            var lambda = Expression.Lambda<Func<TRow, bool>>(joinedConditions, paramExpr);
            return items.Materialize(lambda.Compile());
        }
    }
}