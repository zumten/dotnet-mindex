using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using ZumtenSoft.Mindex.Stubs.IndianCustoms;

namespace ZumtenSoft.Mindex.Benchmark.IndianCustoms
{
    public abstract class IndianCustomsImportBenchmark
    {
        public Import[] Imports { get; set; }
        public ImportTable Table { get; set; }

        [GlobalSetup]
        public virtual void Setup()
        {
            Imports = IndianCustomsHelper.LoadImports();
            Table = new ImportTable(Imports);
        }
    }
}