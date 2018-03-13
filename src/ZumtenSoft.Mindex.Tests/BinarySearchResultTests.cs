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
        private readonly SiteRanking[] _sample =
        {
            new SiteRanking { GlobalRank = 1, DomainName = "google.com" },
            new SiteRanking { GlobalRank = 2, DomainName = "facebook.com" },
            new SiteRanking { GlobalRank = 3, DomainName = "youtube.com" },
            new SiteRanking { GlobalRank = 4, DomainName = "twitter.com" },
            new SiteRanking { GlobalRank = 5, DomainName = "microsoft.com" },
            new SiteRanking { GlobalRank = 6, DomainName = "linkedin.com" },
            new SiteRanking { GlobalRank = 7, DomainName = "wikipedia.org" },
            new SiteRanking { GlobalRank = 8, DomainName = "plus.google.com" },
            new SiteRanking { GlobalRank = 9, DomainName = "apple.com" },
            new SiteRanking { GlobalRank = 10, DomainName = "instagram.com" },
        };

        [TestMethod]
        public void TestConstructor_WhenInitializing_ShouldHaveSameCount()
        {
            BinarySearchResult<SiteRanking> initial = new BinarySearchResult<SiteRanking>(_sample);
            Assert.AreEqual(_sample.Length, initial.Count);
        }

        [TestMethod]
        public void TestReduceIn_WhenFiltering_ShouldReturnOnlyMatchingResults()
        {
            BinarySearchResult<SiteRanking> initial = new BinarySearchResult<SiteRanking>(_sample.OrderBy(s => s.DomainName).ToArray());
            var search = new[] { "microsoft.com", "apple.com", "google.com" };
            var found = initial.ReduceIn(r => r.DomainName, search, Comparer<string>.Default);
            Assert.AreEqual(search.Length, found.Count);
        }

        [TestMethod]
        public void TestReduceRange_WhenFiltering_ShouldReturnOnlyMatchingResults()
        {
            BinarySearchResult<SiteRanking> initial = new BinarySearchResult<SiteRanking>(_sample);
            var found = initial.ReduceRange(r => r.GlobalRank, 3, 7, Comparer<int>.Default);
            Assert.AreEqual(5, found.Count);
        }
    }
}
