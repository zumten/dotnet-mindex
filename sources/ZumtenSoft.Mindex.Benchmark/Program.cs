using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using ZumtenSoft.Mindex.Benchmark.Benchmarks;

namespace ZumtenSoft.Mindex.Benchmark
{

    class Program
    {
        static void Main(string[] args)
        {
            // BenchmarkRunner.Run<SiteRankingSearchTopDomainByTLD>();
            // BenchmarkRunner.Run<SiteRankingSearchTopDomainByComOrgNet>();
            BenchmarkRunner.Run<SiteRankingSearchCanadianDomainInGlobalTop1000>();
            Console.ReadLine();
        }

  
    }
}
