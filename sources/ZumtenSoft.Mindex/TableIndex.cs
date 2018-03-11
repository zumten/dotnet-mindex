using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex
{
    public class TableIndex<TRow, TSearch>
    {
        private readonly Table<TRow, TSearch> _table;
        private readonly ITableColumn<TRow, TSearch>[] _tableColumns;
        private readonly BinarySearchResult<TRow> _items;

        public TableIndex(Table<TRow, TSearch> table, IEnumerable<TRow> items, ITableColumn<TRow, TSearch>[] tableColumns)
        {
            _table = table;
            _tableColumns = tableColumns;

            foreach (var criteria in tableColumns)
                items = criteria.Sort(items);

            _items = new BinarySearchResult<TRow>(items.ToArray());
        }

        public IEnumerable<TRow> Search(TSearch search)
        {
            var binaryResult = _items;
            List<ITableColumn<TRow, TSearch>> processedColumns = new List<ITableColumn<TRow, TSearch>>();
            foreach (var column in _tableColumns)
            {
                if (!binaryResult.CanSearch || !column.Reduce(search, ref binaryResult))
                    break;
                processedColumns.Add(column);
            }

            var remainingColumns = _table.Columns.Except(processedColumns).ToList();
            return FilterRemainingColumns(binaryResult, search, remainingColumns);
        }

        private static IEnumerable<TRow> FilterRemainingColumns(IEnumerable<TRow> items, TSearch search,
            IEnumerable<ITableColumn<TRow, TSearch>> columns)
        {
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

        public float GetScore(TSearch search)
        {
            float score = 0;
            foreach (var column in _tableColumns)
            {
                var columnScore = column.GetScore(search);
                score += columnScore.Item1;
                if (!columnScore.Item2)
                    break;
            }

            return score;
        }
    }

}