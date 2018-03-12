using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<SiteRanking> rankings = null; //LoadRankings();
            SiteRankingTable table = new SiteRankingTable(rankings);

            var top10OfSearchTLD = table.Search(new SiteRankingSearch
            {
                TopLevelDomainRank = SearchCriteria.ByRange(1, 10)
            });

            var top1000OfMainDomains = table.Search(new SiteRankingSearch
            {
                TopLevelDomain = new[] { "com", "org", "net" },
                TopLevelDomainRank = SearchCriteria.ByRange(1, 1000)
            });

         
            var canadianDomainsInTop1000 = table.Search(new SiteRankingSearch
            {
                TopLevelDomain = "ca",
                GlobalRank = SearchCriteria.ByRange(1, 1000)
            });
        }
    }
}
