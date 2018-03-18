using System;
using System.Collections.Generic;
using System.Text;
using ZumtenSoft.Mindex.ColumnCriterias;

namespace ZumtenSoft.Mindex.Columns
{
    public class TableColumnCollection<TRow, TSearch> : List<ITableColumn<TRow, TSearch>>
    {
        public List<ITableCriteriaForColumn<TRow, TSearch>> ExtractCriterias(TSearch search)
        {
            List<ITableCriteriaForColumn<TRow, TSearch>> criterias = new List<ITableCriteriaForColumn<TRow, TSearch>>();
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
