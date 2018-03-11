using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Benchmark
{
    //[ClrJob(isBaseline: true), CoreJob, MonoJob]
    //[RPlotExporter, RankColumn]
    //[DryJob]
    public class BenchmarkSiteRanking
    {
        private List<SiteRanking> _rankings;
        private SiteRankingTable _table;

        [Params(1000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _rankings = MajesticMillionCache.Instance;
            _table = new SiteRankingTable(_rankings);
        }

        [Benchmark]
        public List<SiteRanking> LinqWhere() => _rankings.Where(r => String.Equals(r.TopLevelDomain, "ca", StringComparison.OrdinalIgnoreCase) && r.TopLevelDomainRank >= 1 && r.TopLevelDomainRank <= 2000).ToList();

        [Benchmark]
        public List<SiteRanking> LinqMultiWhere() => _rankings.Where(r => String.Equals(r.TopLevelDomain, "ca", StringComparison.OrdinalIgnoreCase)).Where(r => r.TopLevelDomainRank >= 1 && r.TopLevelDomainRank <= 2000).ToList();

        [Benchmark]
        public List<SiteRanking> SearchFullScan() => _table.IndexFullScan.Search(new SiteRankingSearch { TopLevelDomain = "ca", TopLevelDomainRank = SearchCriteria.ByRange(1, 2000) }).ToList();

        [Benchmark]
        public List<SiteRanking> SearchIndex() => _table.IndexTopLevelDomain.Search(new SiteRankingSearch { TopLevelDomain = "ca", TopLevelDomainRank = SearchCriteria.ByRange(1, 2000) }).ToList();
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSiteRanking bench = new BenchmarkSiteRanking();
            bench.Setup();
            var transat = MajesticMillionCache.Instance.Where(x => x.DomainName.Contains("transat")).ToList();
            var searchWithIndex = bench.SearchIndex();
            BenchmarkRunner.Run<BenchmarkSiteRanking>();
            Console.ReadLine();
        }

  
    }
}
