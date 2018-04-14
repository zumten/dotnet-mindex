using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Tests.Stubs;
using ZumtenSoft.Mindex.Tests.Stubs.MajesticMillion;

namespace ZumtenSoft.Mindex.Benchmark.Benchmarks
{
    //[ClrJob(isBaseline: true), CoreJob, MonoJob]
    //[RPlotExporter, RankColumn]
    //[DryJob]

    public class SiteRankingBenchmark
    {
        public List<SiteRanking> Rankings { get; set; }
        public SiteRankingTable Table { get; set; }
        public ILookup<string, SiteRanking> LookupRankingsByTLD { get; set; }

        [Params(1000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            Rankings = MajesticMillionCache.Instance;
            Table = new SiteRankingTable(Rankings);
            LookupRankingsByTLD = Rankings.ToLookup(r => r.TopLevelDomain, StringComparer.OrdinalIgnoreCase);
        }
    }
}
