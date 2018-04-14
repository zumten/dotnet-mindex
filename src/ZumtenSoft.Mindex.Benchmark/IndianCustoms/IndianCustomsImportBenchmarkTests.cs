using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZumtenSoft.Mindex.Benchmark.IndianCustoms
{
    [TestClass]
    public class IndianCustomsImportBenchmarkTests
    {
        [TestMethod]
        public void SearchByOriginDestinationTypeDate_ValidateData()
        {
            SearchByOriginDestinationTypeDate benchmark = new SearchByOriginDestinationTypeDate();
            benchmark.Setup();

            var linq = benchmark.Linq();
            var lookup = benchmark.Lookup();
            var lookupWithBinarySearch = benchmark.LookupWithBinarySearch();
            var search = benchmark.Search();

            CollectionAssert.AreEquivalent(linq, lookup);
            CollectionAssert.AreEquivalent(linq, lookupWithBinarySearch);
            CollectionAssert.AreEquivalent(linq, search);
        }

        [TestMethod]
        public void SearchByOriginDestinationTypeDate_Start()
        {
            BenchmarkRunner.Run<SearchByOriginDestinationTypeDate>();
        }

        [TestMethod]
        public void SearchBySingleOrigin_ValidateData()
        {
            SearchBySingleOrigin benchmark = new SearchBySingleOrigin();
            benchmark.Setup();

            var linq = benchmark.Linq();
            var lookup = benchmark.Lookup();
            var search = benchmark.Search();

            CollectionAssert.AreEquivalent(linq, lookup);
            CollectionAssert.AreEquivalent(linq, search);
        }

        [TestMethod]
        public void SearchBySingleOrigin_Start()
        {
            BenchmarkRunner.Run<SearchBySingleOrigin>();
        }


        [TestMethod]
        public void SearchByMultipleOrigins_ValidateData()
        {
            SearchByMultipleOrigins benchmark = new SearchByMultipleOrigins();
            benchmark.Setup();

            var linq = benchmark.Linq();
            var lookup = benchmark.Lookup();
            var search = benchmark.Search();

            CollectionAssert.AreEquivalent(linq, lookup);
            CollectionAssert.AreEquivalent(linq, search);
        }

        [TestMethod]
        public void SearchByMultipleOrigins_Start()
        {
            BenchmarkRunner.Run<SearchByMultipleOrigins>();
        }
    }
}
