using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Indexes;
using ZumtenSoft.Mindex.Mappings;
using ZumtenSoft.Mindex.Utilities;

namespace ZumtenSoft.Mindex
{
    public abstract class Table<TRow, TSearch>
    {
        private readonly TableMappingCollection<TRow, TSearch> _mappings;

        private readonly TableIndexCollection<TRow, TSearch> _indexes;

        protected Table(IReadOnlyCollection<TRow> rows)
        {
            _mappings = new TableMappingCollection<TRow, TSearch>();
            _indexes = new TableIndexCollection<TRow, TSearch>(rows);
        }

        public TableMappingConfigurator<TRow, TSearch, TColumn, SearchCriteria<TColumn>> MapCriteria<TColumn>(Expression<Func<TSearch, SearchCriteria<TColumn>>> getCriteriaExpression)
        {
            return new TableMappingConfigurator<TRow, TSearch, TColumn, SearchCriteria<TColumn>>(_mappings, _indexes.DefaultIndex.Rows, getCriteriaExpression);
        }

        public TableMappingByValueConfigurator<TRow, TSearch, TColumn> MapCriteria<TColumn>(Expression<Func<TSearch, SearchCriteriaByValue<TColumn>>> getCriteriaExpression)
        {
            return new TableMappingByValueConfigurator<TRow, TSearch, TColumn>(_mappings, _indexes.DefaultIndex.Rows, getCriteriaExpression);
        }

        /// <summary>
        /// Start configuring an index using the builder pattern.
        /// </summary>
        protected TableIndexConfigurator<TRow, TSearch> ConfigureIndex()
        {
            return new TableIndexConfigurator<TRow, TSearch>(_indexes, _mappings);
        }

        /// <summary>
        /// Search the table using the search object passed in parameter. The search will
        /// be executed using the specified index or the index that best matches the criterias.
        /// </summary>
        /// <param name="search">Search containing the list of criterias to apply</param>
        /// <param name="index">Optionally force an index</param>
        /// <returns></returns>
        public TRow[] Search(TSearch search, TableIndex<TRow, TSearch> index = null)
        {
            var criterias = _mappings.ExtractCriterias(search);

            if (index != null)
                return index.Search(criterias);

            var bestIndex = _indexes.GetBestIndex(criterias);
            // Null is returned when any of the index find that this search will return
            // an empty result set.
            if (bestIndex == null)
                return ArrayUtilities<TRow>.EmptyArray;
            return bestIndex.Search(criterias);
        }

        public TRow[] Search(TSearch[] criterias, bool distinct = false)
        {
            switch (criterias.Length)
            {
                case 0: return ArrayUtilities<TRow>.EmptyArray;
                case 1: return Search(criterias[0]);
                default:
                {
                    if (distinct)
                    {
                        HashSet<TRow> rows = new HashSet<TRow>();
                        foreach (var criteria in criterias)
                        {
                            var result = Search(criteria);
                            var length = result.Length;
                            for (int i = 0; i < length; i++)
                                rows.Add(result[i]);
                        }

                        return rows.ToArray();
                    }

                    TRow[][] results = new TRow[criterias.Length][];
                    for (int i = 0; i < criterias.Length; i++)
                        results[i] = Search(criterias[i]);

                    return ArrayUtilities<TRow>.Flatten(results);
                }
            }
        }

        public TableIndexScore<TRow, TSearch>[] EvaluateIndexes(TSearch search)
        {
            var criterias = _mappings.ExtractCriterias(search);
            return _indexes.EvaluateIndexes(criterias);
        }
    }
}