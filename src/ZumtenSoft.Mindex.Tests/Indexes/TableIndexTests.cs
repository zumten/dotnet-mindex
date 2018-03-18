using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Tests.Indexes
{
    [TestClass]
    public class TableIndexTests
    {
        private static SiteRankingTable _table;
        private static List<SiteRanking> _rows;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _rows = MajesticMillionHelper.LoadSiteRankings(@"App_Data\majestic_million_reduced.csv").ToList();
            _table = new SiteRankingTable(_rows);
        }

        [TestMethod]
        public void Search_FilteringByCanadianSitesInTop1000_ShouldBeEquivalentToLinqs()
        {
            const string topLevelDomain = "ca";
            const int globalRank = 1000;

            var expected = _rows.Where(x => x.TopLevelDomain == topLevelDomain && x.GlobalRank <= globalRank).ToList();
            var actual = _table.Search(new SiteRankingSearch
            {
                TopLevelDomain = topLevelDomain,
                GlobalRank = SearchCriteria.ByRange(1, globalRank)
            }).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Search_FilteringBySiteRankingSearchTopDomainByComOrgNet_ShouldBeEquivalentToLinq()
        {
            var topLevelDomains = new[] {"com", "net", "org "};
            const int topLevelDomainRank = 1000;

            var expected = _rows.Where(x => topLevelDomains.Contains(x.TopLevelDomain) && x.TopLevelDomainRank <= topLevelDomainRank).ToList();
            var actual = _table.Search(new SiteRankingSearch {
                TopLevelDomain = topLevelDomains,
                TopLevelDomainRank = SearchCriteria.ByRange(1, topLevelDomainRank)
            }).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void Search_FilteringByTopDomainByTLD_ShouldBeEquivalentToLinq()
        {
            const int topLevelDomainRank = 10;

            var expected = _rows.Where(x => x.TopLevelDomainRank <= topLevelDomainRank).ToList();
            var actual = _table.Search(new SiteRankingSearch
            {
                TopLevelDomainRank = SearchCriteria.ByRange(1, topLevelDomainRank)
            }).ToList();

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}
