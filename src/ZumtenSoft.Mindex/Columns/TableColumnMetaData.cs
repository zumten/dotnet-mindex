using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ZumtenSoft.Mindex.Columns
{
    public class TableColumnMetaData<TRow, TColumn>
    {
        public IComparer<TColumn> Comparer { get; }
        public IEqualityComparer<TColumn> EqualityComparer { get; }
        public Func<TRow, TColumn> GetColumnValue { get; }
        public Expression<Func<TRow, TColumn>> GetColumnExpression { get; }
        public TColumn[] PossibleValues { get; }

        public TableColumnMetaData(IEnumerable<TRow> rows, IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer, Expression<Func<TRow, TColumn>> getColumnExpression)
        {
            Comparer = comparer;
            EqualityComparer = equalityComparer;
            GetColumnExpression = getColumnExpression;
            GetColumnValue = getColumnExpression.Compile();
            PossibleValues = rows
                .Select(GetColumnValue)
                .Distinct(equalityComparer)
                .OrderBy(x => x, comparer)
                .ToArray();
        }
    }
}
