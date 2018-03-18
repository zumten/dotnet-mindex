using BenchmarkDotNet.Running;
using System;
using ZumtenSoft.Mindex.Benchmark.Benchmarks;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var rows = MajesticMillionCache.Instance;
            BenchmarkRunner.Run<SiteRankingSearchTopDomainByTLD>();
            BenchmarkRunner.Run<SiteRankingSearchTopDomainByComOrgNet>();
            BenchmarkRunner.Run<SiteRankingSearchCanadianDomainInGlobalTop1000>();
            Console.ReadLine();
        }
    }
}
