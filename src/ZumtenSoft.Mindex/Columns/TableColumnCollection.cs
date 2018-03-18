using System;
using System.Collections.Generic;
using System.Text;
using ZumtenSoft.Mindex.ColumnCriterias;

namespace ZumtenSoft.Mindex.Columns
{
    public class TableColumnCollection<TRow, TSearch> : List<ITableColumn<TRow, TSearch>>
    {
        public List<ITableColumnCriteria<TRow, TSearch>> ExtractCriterias(TSearch search)
        {
            List<ITableColumnCriteria<TRow, TSearch>> criterias = new List<ITableColumnCriteria<TRow, TSearch>>();
            foreach (var column in this)
            {
                var criteria = column.ExtractCriteria(search);
                if (criteria != null)
                    criterias.Add(criteria);
            }

            return criterias;
        }
    }
}
