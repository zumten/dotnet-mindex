using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Tests
{
    [TestClass]
    public class TableTests
    {
        private static SiteRankingTable _table;
        private static SiteRanking[] _rows;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _rows = SiteRankingCollections.First10000Rows;
            _table = new SiteRankingTable(_rows);
        }

        [TestMethod]
        public void Search_FilteringByCanadianSitesInTop1000_ShouldBeEquivalentToLinqs()
        {
            const string topLevelDomain = "ca";
            const int globalRank = 1000;

            var expected = _rows.Where(x => x.TopLevelDomain == topLevelDomain && x.GlobalRank <= globalRank).ToList();
            var search = new SiteRankingSearch
            {
                TopLevelDomain = topLevelDomain,
                GlobalRank = SearchCriteria.ByRange(1, globalRank)
            };
            var actual = _table.Search(search).ToList();
            CollectionAssert.AreEquivalent(expected, actual);

            var actualWithGlobalIndex = _table.Search(search, _table.IndexGlobalRank).ToList();
            CollectionAssert.AreEquivalent(expected, actualWithGlobalIndex);
        }

        [TestMethod]
        public void Search_FilteringBySiteRankingSearchTopDomainByComOrgNet_ShouldBeEquivalentToLinq()
        {
            var topLevelDomains = new[] {"com", "net", "org "};
            const int topLevelDomainRank = 1000;

            var expected = _rows.Where(x => topLevelDomains.Contains(x.TopLevelDomain) && x.TopLevelDomainRank <= topLevelDomainRank).ToList();
            var search = new SiteRankingSearch
            {
                TopLevelDomain = topLevelDomains,
                TopLevelDomainRank = SearchCriteria.ByRange(1, topLevelDomainRank)
            };
            var actual = _table.Search(search).ToList();
            var indexScores = _table.EvaluateIndexes(search);
            CollectionAssert.AreEquivalent(expected, actual);

            var searchWithPredicate = new SiteRankingSearch
            {
                TopLevelDomain = topLevelDomains,
                TopLevelDomainRank = SearchCriteria.ByPredicate((int x) => x <= topLevelDomainRank)
            };
            var actualWithPredicate = _table.Search(searchWithPredicate).ToList();
            var indexScoreWithPredicate = _table.EvaluateIndexes(searchWithPredicate);
            CollectionAssert.AreEquivalent(expected, actualWithPredicate);

            Assert.IsTrue(indexScores[0].Score < indexScoreWithPredicate[0].Score, "Search without predicate should have a better score");
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
