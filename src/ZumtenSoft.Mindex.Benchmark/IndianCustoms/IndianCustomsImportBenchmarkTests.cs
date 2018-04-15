using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZumtenSoft.Mindex.Benchmark.IndianCustoms
{
#if !DEBUG
    [TestClass]
#endif
    public class IndianCustomsImportBenchmarkTests
    {
        [TestMethod]
        public void SearchByOriginDestinationQuantityTypeDateBenchmark()
        {
            BenchmarkRunner.Run<SearchByOriginDestinationQuantityTypeDate>();
        }

        [TestMethod]
        public void SearchBySingleOriginBenchmark()
        {
            BenchmarkRunner.Run<SearchBySingleOrigin>();
        }
        
        [TestMethod]
        public void SearchByMultipleOriginsBenchmark()
        {
            BenchmarkRunner.Run<SearchByMultipleOrigins>();
        }
    }
}
