using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Tests
{
    [TestClass]
    public class BinarySearchResultTests
    {


        [TestMethod]
        public void TestConstructor_WhenInitializing_ShouldHaveSameCount()
        {
            BinarySearchResult<SiteRanking> initial = new BinarySearchResult<SiteRanking>(SiteRankingCollections.FirstTenRows);
            Assert.AreEqual(SiteRankingCollections.FirstTenRows.Length, initial.Count);
        }

        [TestMethod]
        public void TestReduceIn_WhenFiltering_ShouldReturnOnlyMatchingResults()
        {
            BinarySearchResult<SiteRanking> initial = new BinarySearchResult<SiteRanking>(SiteRankingCollections.FirstTenRows.OrderBy(s => s.DomainName).ToArray());
            var search = new[] { "microsoft.com", "apple.com", "google.com" };
            var found = initial.ReduceIn(r => r.DomainName, search, Comparer<string>.Default);
            Assert.AreEqual(search.Length, found.Count);
        }

        [TestMethod]
        public void TestReduceRange_WhenFiltering_ShouldReturnOnlyMatchingResults()
        {
            BinarySearchResult<SiteRanking> initial = new BinarySearchResult<SiteRanking>(SiteRankingCollections.FirstTenRows);
            var found = initial.ReduceRange(r => r.GlobalRank, 3, 7, Comparer<int>.Default);
            Assert.AreEqual(5, found.Count);
        }
    }
}
