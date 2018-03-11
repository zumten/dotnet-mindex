using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZumtenSoft.Mindex.Tests.Stubs
{
    public class SiteRankingTable : Table<SiteRanking, SiteRankingSearch>
    {
        public SiteRankingTable(IReadOnlyCollection<SiteRanking> rankings)
        {
            var columnGlobalRank = MapSearchCriteria(c => c.GlobalRank, r => r.GlobalRank);
            var columnTopLevelDomainRank = MapSearchCriteria(c => c.TopLevelDomainRank, r => r.TopLevelDomainRank);
            var columnDomainName = MapSearchCriteria(c => c.DomainName, r => r.DomainName, StringComparer.OrdinalIgnoreCase);
            var columnTopLevelDomain = MapSearchCriteria(c => c.TopLevelDomain, r => r.TopLevelDomain, StringComparer.OrdinalIgnoreCase);
            var columnReferringSubNets = MapSearchCriteria(c => c.ReferringSubNets, r => r.ReferringSubNets);
            var columnReferringIps = MapSearchCriteria(c => c.ReferringIps, r => r.ReferringIps);

            IndexFullScan = BuildIndex(rankings);
            IndexTopLevelDomain = BuildIndex(rankings, columnTopLevelDomain, columnTopLevelDomainRank);
            IndexGlobalRank = BuildIndex(rankings, columnGlobalRank);
        }

        public TableIndex<SiteRanking, SiteRankingSearch> IndexFullScan { get; }
        public TableIndex<SiteRanking, SiteRankingSearch> IndexTopLevelDomain { get; }
        public TableIndex<SiteRanking, SiteRankingSearch> IndexGlobalRank { get; }
    }
}
