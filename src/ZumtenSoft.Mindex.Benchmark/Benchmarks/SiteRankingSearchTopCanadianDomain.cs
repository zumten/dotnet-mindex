using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Tests.Stubs;

namespace ZumtenSoft.Mindex.Benchmark.Benchmarks
{
    public class SiteRankingSearchCanadianDomainInGlobalTop1000 : SiteRankingBenchmark
    {
        private static string CriteriaTopLevelDomain = "ca";
        private static int CriteriaGlobalRank = 1000;
        private static readonly SiteRankingSearch SearchCriterias = new SiteRankingSearch { TopLevelDomain = CriteriaTopLevelDomain, GlobalRank = SearchCriteria.ByRange(1, CriteriaGlobalRank) };

        [Benchmark]
        public List<SiteRanking> Linq() => Rankings.Where(r => r.GlobalRank <= CriteriaGlobalRank && String.Equals(r.TopLevelDomain, CriteriaTopLevelDomain, StringComparison.OrdinalIgnoreCase)).ToList();

        [Benchmark]
        public List<SiteRanking> Lookup() => LookupRankingsByTLD[CriteriaTopLevelDomain].TakeWhile(r => r.GlobalRank <= CriteriaGlobalRank).ToList();

        [Benchmark]
        public SiteRanking[] Search() => Table.Search(SearchCriterias);

        [Benchmark]
        public SiteRanking[] IndexGlobalRank() => Table.Search(SearchCriterias, Table.IndexGlobalRank);

        [Benchmark]
        public SiteRanking[] IndexTopLevelDomain() => Table.Search(SearchCriterias, Table.IndexTopLevelDomain);

        [Benchmark]
        public SiteRanking[] IndexTopLevelDomainGlobalRank() => Table.Search(SearchCriterias, Table.IndexTopLevelDomainGlobalRank);
    }
}
