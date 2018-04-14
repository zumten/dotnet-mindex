using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Stubs.MajesticMillion;

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

        [TestMethod]
        public void TestReduceRangeByValue_WhenFiltering_ShouldReturnOneSegmentForEachValue()
        {
            BinarySearchResult<SiteRanking> initial = new BinarySearchResult<SiteRanking>(SiteRankingCollections.FirstTenRows);
            var found = initial.ReduceRangeByValue(r => r.GlobalRank, 3, 7, Comparer<int>.Default);
            Assert.AreEqual(5, found.Count);
            Assert.AreEqual(5, found.Segments.Length);
        }

        [TestMethod]
        public void TestReduceRangeByValue_WhenFilteringLongList_ShouldReturnSameResultsAsRange()
        {
            BinarySearchResult<SiteRanking> initial = new BinarySearchResult<SiteRanking>(SiteRankingCollections.First10000Rows.OrderBy(x => x.TopLevelDomain).ToArray());
            var foundRange = initial.ReduceRange(r => r.TopLevelDomain, "ca", "net", StringComparer.OrdinalIgnoreCase);
            var foundRangeByValue = initial.ReduceRangeByValue(r => r.TopLevelDomain, "ca", "net", StringComparer.OrdinalIgnoreCase);
            CollectionAssert.AreEquivalent(foundRange.ToList(), foundRangeByValue.ToList());
            Assert.AreEqual(foundRange.ToLookup(x => x.TopLevelDomain).Count, foundRangeByValue.Segments.Length);
        }
    }
}
