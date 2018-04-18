using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ZumtenSoft.Mindex.Mappings
{
    public class TableMappingMetaData<TRow, TColumn>
    {
        public IComparer<TColumn> Comparer { get; }
        public IEqualityComparer<TColumn> EqualityComparer { get; }
        public Func<TRow, TColumn> GetTargetValue { get; }
        public Expression<Func<TRow, TColumn>> GetTargetExpression { get; }
        public TColumn[] PossibleValues { get; }

        public TableMappingMetaData(IEnumerable<TRow> rows, IComparer<TColumn> comparer, IEqualityComparer<TColumn> equalityComparer, Expression<Func<TRow, TColumn>> getTargetExpression)
        {
            Comparer = comparer;
            EqualityComparer = equalityComparer;
            GetTargetExpression = getTargetExpression;
            GetTargetValue = getTargetExpression.Compile();
            PossibleValues = rows
                .Select(GetTargetValue)
                .Distinct(equalityComparer)
                .OrderBy(x => x, comparer)
                .ToArray();
        }
    }
}
