using System;
using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes.Jobs;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Stubs.IndianCustoms;

namespace ZumtenSoft.Mindex.Benchmark.Benchmarks
{
    //[ClrJob(isBaseline: true), CoreJob, MonoJob]
    //[RPlotExporter, RankColumn]
    //[DryJob]
    public class IndianCustomsImportBenchmark
    {
        public List<CustomsImport> Imports { get; set; }
        public CustomsImportTable Table { get; set; }
        public ILookup<Tuple<string, string, string>, CustomsImport> LookupTable { get; set; }
        public IDictionary<Tuple<string, string, string>, CustomsImport[]> LookupWithBinarySearchTable { get; set; }

        [Params(1000)] public int N;

        [GlobalSetup]
        public void Setup()
        {
            Imports = CustomsImportsCache.Instance;
            Table = new CustomsImportTable(Imports);
            LookupTable = Imports.OrderBy(i => i.Date).ToLookup(i => Tuple.Create(i.Origin, i.ImportState, i.QuantityType));
            LookupWithBinarySearchTable = LookupTable
                .ToDictionary(g => g.Key, g => g.ToArray());
        }
    }
}