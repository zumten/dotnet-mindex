using System.Linq;

namespace ZumtenSoft.Mindex.Tests.Stubs
{
    public static class SiteRankingCollections
    {
        public static SiteRanking[] FirstTenRows =
        {
            new SiteRanking { GlobalRank = 1, DomainName = "google.com", TopLevelDomain = "com" },
            new SiteRanking { GlobalRank = 2, DomainName = "facebook.com", TopLevelDomain = "com" },
            new SiteRanking { GlobalRank = 3, DomainName = "youtube.com", TopLevelDomain = "com" },
            new SiteRanking { GlobalRank = 4, DomainName = "twitter.com", TopLevelDomain = "com" },
            new SiteRanking { GlobalRank = 5, DomainName = "microsoft.com", TopLevelDomain = "com" },
            new SiteRanking { GlobalRank = 6, DomainName = "linkedin.com", TopLevelDomain = "com" },
            new SiteRanking { GlobalRank = 7, DomainName = "wikipedia.org", TopLevelDomain = "org" },
            new SiteRanking { GlobalRank = 8, DomainName = "plus.google.com", TopLevelDomain = "com" },
            new SiteRanking { GlobalRank = 9, DomainName = "apple.com", TopLevelDomain = "com" },
            new SiteRanking { GlobalRank = 10, DomainName = "instagram.com", TopLevelDomain = "com" },
        };

        private static SiteRanking[] _instance;

        public static SiteRanking[] First10000Rows => _instance ?? (_instance = MajesticMillionHelper.LoadSiteRankings(@"App_Data\majestic_million_reduced.csv").ToArray());
    }
}
