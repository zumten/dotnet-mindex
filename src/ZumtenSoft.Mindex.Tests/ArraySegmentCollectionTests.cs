using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Stubs.MajesticMillion;

namespace ZumtenSoft.Mindex.Tests
{
    [TestClass]
    public class ArraySegmentCollectionTests
    {
        [TestMethod]
        public void TestConstructor_WhenInitializing_ShouldHaveSameCount()
        {
            ArraySegmentCollection<SiteRanking> initial = new ArraySegmentCollection<SiteRanking>(SiteRankingCollections.FirstTenRows);
            Assert.AreEqual(SiteRankingCollections.FirstTenRows.Length, initial.Count);
        }

        [TestMethod]
        public void TestReduceIn_WhenFiltering_ShouldReturnOnlyMatchingResults()
        {
            ArraySegmentCollection<SiteRanking> initial = new ArraySegmentCollection<SiteRanking>(SiteRankingCollections.FirstTenRows.OrderBy(s => s.DomainName).ToArray());
            var search = new[] { "microsoft.com", "apple.com", "google.com" };
            var found = initial.ReduceByValues(r => r.DomainName, search, Comparer<string>.Default);
            Assert.AreEqual(search.Length, found.Count);
        }

        [TestMethod]
        public void TestReduceRange_WhenFiltering_ShouldReturnOnlyMatchingResults()
        {
            ArraySegmentCollection<SiteRanking> initial = new ArraySegmentCollection<SiteRanking>(SiteRankingCollections.FirstTenRows);
            var found = initial.ReduceByRange(r => r.GlobalRank, 3, 7, Comparer<int>.Default);
            Assert.AreEqual(5, found.Count);
        }

        [TestMethod]
        public void TestReduceByRangePreserveSearchability_WhenFiltering_ShouldReturnOneSegmentForEachValue()
        {
            ArraySegmentCollection<SiteRanking> initial = new ArraySegmentCollection<SiteRanking>(SiteRankingCollections.FirstTenRows);
            var found = initial.ReduceByRange(r => r.GlobalRank, 3, 7, Comparer<int>.Default, true);
            Assert.AreEqual(5, found.Count);
            Assert.AreEqual(5, found.Segments.Length);
        }

        [TestMethod]
        public void TestReduceByRangePreserveSearchability_WhenFilteringLongList_ShouldReturnSameResultsAsRange()
        {
            ArraySegmentCollection<SiteRanking> initial = new ArraySegmentCollection<SiteRanking>(SiteRankingCollections.First10000Rows.OrderBy(x => x.TopLevelDomain).ToArray());
            var foundRange = initial.ReduceByRange(r => r.TopLevelDomain, "ca", "net", StringComparer.OrdinalIgnoreCase);
            var foundRangeByValue = initial.ReduceByRange(r => r.TopLevelDomain, "ca", "net", StringComparer.OrdinalIgnoreCase, true);
            CollectionAssert.AreEquivalent(foundRange.ToList(), foundRangeByValue.ToList());
            Assert.AreEqual(foundRange.ToLookup(x => x.TopLevelDomain).Count, foundRangeByValue.Segments.Length);
        }
    }
}
