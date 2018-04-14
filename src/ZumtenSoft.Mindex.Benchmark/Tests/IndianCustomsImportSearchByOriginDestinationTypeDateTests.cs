using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZumtenSoft.Mindex.Benchmark.Benchmarks;

namespace ZumtenSoft.Mindex.Benchmark.Tests
{
    [TestClass]
    public class IndianCustomsImportSearchByOriginDestinationTypeDateTests
    {
        [TestMethod]
        public void ValidateData()
        {
            IndianCustomsImportSearchByOriginDestinationTypeDate benchmark = new IndianCustomsImportSearchByOriginDestinationTypeDate();
            benchmark.Setup();

            var linq = benchmark.Linq();
            var lookup = benchmark.Lookup();
            var lookupWithBinarySearch = benchmark.LookupWithBinarySearch();
            var search = benchmark.Search();

            CollectionAssert.AreEquivalent(linq, lookup);
            CollectionAssert.AreEquivalent(linq, lookupWithBinarySearch);
            CollectionAssert.AreEquivalent(linq, search);
        }
    }
}
