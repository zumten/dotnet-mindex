using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Benchmark.Benchmarks
{
    public class SiteRankingSearchTopDomainByComOrgNet : SiteRankingBenchmark
    {
        private static readonly string[] Domains = { "com", "net", "org" };

        [Benchmark]
        public List<SiteRanking> Linq() => Rankings.Where(r => Domains.Contains(r.DomainName, StringComparer.OrdinalIgnoreCase) && r.TopLevelDomainRank >= 1 && r.TopLevelDomainRank <= 1000).ToList();

        [Benchmark]
        public List<SiteRanking> Lookup() => Domains.SelectMany(d => LookupRankingsByTLD[d].Take(1000)).ToList();

        [Benchmark]
        public List<SiteRanking> Search() => Table.Search(new SiteRankingSearch { TopLevelDomain = Domains, TopLevelDomainRank = SearchCriteria.ByRange(1, 1000) }).ToList();
    }
}
