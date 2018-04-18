using System.Collections.Generic;
using ZumtenSoft.Mindex.MappingCriterias;

namespace ZumtenSoft.Mindex.Mappings
{
    public class TableMappingCollection<TRow, TSearch> : List<ITableMapping<TRow, TSearch>>
    {
        public List<ITableCriteriaForMapping<TRow, TSearch>> ExtractCriterias(TSearch search)
        {
            List<ITableCriteriaForMapping<TRow, TSearch>> criterias = new List<ITableCriteriaForMapping<TRow, TSearch>>();
            foreach (var mapping in this)
            {
                var criteria = mapping.ExtractCriteria(search);
                if (criteria != null)
                    criterias.Add(criteria);
            }

            return criterias;
        }
    }
}
