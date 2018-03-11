using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZumtenSoft.Mindex.Columns;

namespace ZumtenSoft.Mindex.Tests.Stubs
{
    public class SiteRankingTable : Table<SiteRanking, SiteRankingSearch>
    {
        public SiteRankingTable(IReadOnlyCollection<SiteRanking> rankings) : base(rankings)
        {
            MapSearchCriteria(s => s.GlobalRank, r => r.GlobalRank);
            MapSearchCriteria(s => s.TopLevelDomainRank, r => r.TopLevelDomainRank);
            MapSearchCriteria(s => s.DomainName, r => r.DomainName, StringComparer.OrdinalIgnoreCase);
            MapSearchCriteria(s => s.TopLevelDomain, r => r.TopLevelDomain, StringComparer.OrdinalIgnoreCase);

            IndexFullScan = BuildIndex(new ITableColumn<SiteRanking, SiteRankingSearch>[0]);
            IndexTopLevelDomain = BuildIndex(s => s.TopLevelDomain);
            IndexGlobalRank = BuildIndex(s => s.GlobalRank);
        }

        public TableIndex<SiteRanking, SiteRankingSearch> IndexFullScan { get; }
        public TableIndex<SiteRanking, SiteRankingSearch> IndexTopLevelDomain { get; }
        public TableIndex<SiteRanking, SiteRankingSearch> IndexGlobalRank { get; }
    }
}
