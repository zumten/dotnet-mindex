using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZumtenSoft.Mindex.Benchmark.IndianCustoms
{
    [TestClass]
    public class IndianCustomsImportBenchmarkValidateDataTests
    {
        [TestMethod]
        public void SearchByOriginDestinationQuantityTypeDate()
        {
            SearchByOriginDestinationQuantityTypeDate benchmark = new SearchByOriginDestinationQuantityTypeDate();
            benchmark.Setup();

            var resultLinq = benchmark.SearchLinq();
            var resultLookup = benchmark.SearchLookup();
            var resultLookupWithBinarySearch = benchmark.SearchLookupWithBinarySearch();
            var resultOrderedList = benchmark.SearchOrderedListWithBinarySearch();
            var resultMindex = benchmark.SearchMindex();

            CollectionAssert.AreEquivalent(resultLinq, resultLookup);
            CollectionAssert.AreEquivalent(resultLinq, resultLookupWithBinarySearch);
            CollectionAssert.AreEquivalent(resultLinq, resultOrderedList);
            CollectionAssert.AreEquivalent(resultLinq, resultMindex);
        }

        [TestMethod]
        public void SearchBySingleOrigin()
        {
            SearchBySingleOrigin benchmark = new SearchBySingleOrigin();
            benchmark.Setup();

            var resultLinq = benchmark.SearchLinq();
            var resultLookup = benchmark.SearchLookup();
            var resultMindex = benchmark.SearchMindex();

            CollectionAssert.AreEquivalent(resultLinq, resultLookup);
            CollectionAssert.AreEquivalent(resultLinq, resultMindex);
        }

        [TestMethod]
        public void SearchByMultipleOrigins()
        {
            SearchByMultipleOrigins benchmark = new SearchByMultipleOrigins();
            benchmark.Setup();

            var resultLinq = benchmark.SearchLinq();
            var resultLookup = benchmark.SearchLookup();
            var resultMindex = benchmark.SearchMindex();

            CollectionAssert.AreEquivalent(resultLinq, resultLookup);
            CollectionAssert.AreEquivalent(resultLinq, resultMindex);
        }
    }
}
