using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Benchmark.Benchmarks
{
    public class SiteRankingSearchTopDomainByTLD : SiteRankingBenchmark
    {
        [Benchmark]
        public List<SiteRanking> Linq() => Rankings.Where(r => r.TopLevelDomainRank >= 1 && r.TopLevelDomainRank <= 10).ToList();

        [Benchmark]
        public List<SiteRanking> Lookup() => LookupRankingsByTLD.SelectMany(g => g.Take(10)).ToList();

        [Benchmark]
        public List<SiteRanking> Search() => Table.Search(new SiteRankingSearch { TopLevelDomainRank = SearchCriteria.ByRange(1, 10) }).ToList();
    }
}
