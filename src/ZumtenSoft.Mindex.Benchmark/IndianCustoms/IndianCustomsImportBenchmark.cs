using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using ZumtenSoft.Mindex.Stubs.IndianCustoms;

namespace ZumtenSoft.Mindex.Benchmark.IndianCustoms
{
    //[ClrJob(isBaseline: true), CoreJob, MonoJob]
    //[RPlotExporter, RankColumn]
    //[DryJob]
    public abstract class IndianCustomsImportBenchmark
    {
        public Import[] Imports { get; set; }
        public ImportTable Table { get; set; }

        [Params(1000)] public int N;

        [GlobalSetup]
        public virtual void Setup()
        {
            Imports = IndianCustomsHelper.LoadImports();
            Table = new ImportTable(Imports);
        }
    }
}