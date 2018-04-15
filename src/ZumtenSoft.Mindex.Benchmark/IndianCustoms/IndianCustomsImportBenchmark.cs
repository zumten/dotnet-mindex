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
        public CustomsImport[] Imports { get; set; }
        public CustomsImportTable Table { get; set; }

        [Params(1000)] public int N;

        [GlobalSetup]
        public virtual void Setup()
        {
            Imports = CustomsImportHelper.LoadCustomImports();
            Table = new CustomsImportTable(Imports);
        }
    }
}