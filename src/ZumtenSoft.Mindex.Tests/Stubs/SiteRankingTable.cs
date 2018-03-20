using System;
using System.Collections.Generic;
using ZumtenSoft.Mindex.Indexes;

namespace ZumtenSoft.Mindex.Tests.Stubs
{
    public class SiteRankingTable : Table<SiteRanking, SiteRankingSearch>
    {
        public SiteRankingTable(IReadOnlyCollection<SiteRanking> rankings) : base(rankings)
        {
            MapCriteria(s => s.GlobalRank, r => r.GlobalRank);
            MapCriteria(s => s.TopLevelDomainRank, r => r.TopLevelDomainRank);
            MapCriteria(s => s.DomainName, r => r.DomainName, StringComparer.OrdinalIgnoreCase);
            MapCriteria(s => s.TopLevelDomain, r => r.TopLevelDomain, StringComparer.OrdinalIgnoreCase);
            MapCriteriaToPredicate(s => s.TLDContainsChar, (r, c) => r.TopLevelDomain.IndexOf(c) >= 0);

            IndexTopLevelDomainRank = ConfigureIndex().IncludeColumns(s => s.TopLevelDomainRank).Build();
            IndexTopLevelDomain = ConfigureIndex().IncludeColumns(s => s.TopLevelDomain, s => s.TopLevelDomainRank).Build();
            IndexGlobalRank = ConfigureIndex().IncludeColumns(s => s.GlobalRank).Build();
            IndexTopLevelDomainGlobalRank = ConfigureIndex().IncludeColumns(s => s.TopLevelDomain, s => s.GlobalRank).Build();
        }

        public TableIndex<SiteRanking, SiteRankingSearch> IndexTopLevelDomainRank { get; set; }
        public TableIndex<SiteRanking, SiteRankingSearch> IndexTopLevelDomain { get; }
        public TableIndex<SiteRanking, SiteRankingSearch> IndexGlobalRank { get; }
        public TableIndex<SiteRanking, SiteRankingSearch> IndexTopLevelDomainGlobalRank { get; }
    }
}
