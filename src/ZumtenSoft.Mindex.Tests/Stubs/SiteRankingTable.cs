using System;
using System.Collections.Generic;
using ZumtenSoft.Mindex.Indexes;

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

            IndexTopLevelDomain = ConfigureIndex().IncludeColumns(s => s.TopLevelDomain, s => s.TopLevelDomainRank).Build();
            IndexGlobalRank = ConfigureIndex().IncludeColumns(s => s.GlobalRank).Build();
            IndexTopLevelDomainGlobalRank = ConfigureIndex().IncludeColumns(s => s.TopLevelDomain, s => s.GlobalRank).Build();
        }

        public TableIndex<SiteRanking, SiteRankingSearch> IndexTopLevelDomain { get; }
        public TableIndex<SiteRanking, SiteRankingSearch> IndexGlobalRank { get; }
        public TableIndex<SiteRanking, SiteRankingSearch> IndexTopLevelDomainGlobalRank { get; }
    }
}
