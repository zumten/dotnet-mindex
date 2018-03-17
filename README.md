# mindex
Mindex: DotNet High-performance in-memory multi-criteria collection search

[![Build Status](https://zumten.visualstudio.com/_apis/public/build/definitions/d6fe51c2-2715-43c8-8bff-5cb5575470b4/1/badge)](https://zumten.visualstudio.com/ZumtenSoft/_build/index?definitionId=1)


## Objective

This library tries to mimmick the way you can search inside a table in SQL through multiple criterias, while providing the full performance of in-memory objects.

## How it works?

The inner workings of Mindex is similar to SQL. Each index is simply a list of items sorted with multiple criterias. The search is then done using BinarySearch for each criteria. When you don't specify which index to use, Mindex will analyze the search, give a score to each index and choose the best one.


## Build your first table

There are 4 simple steps to build your own table with indexes

1. Create your POCO object and a search criteria with a duplicate of every property you need to use as a search criteria

```csharp
public class SiteRanking
{
    public int GlobalRank { get; set; }
    public int TopLevelDomainRank { get; set; }
    public string DomainName { get; set; }
    public string TopLevelDomain { get; set; }
    public int ReferringSubNets { get; set; }
    public int ReferringIps { get; set; }
    public string InternationalizedDomainName { get; set; }
    public string InternationalizedTopLevelDomain { get; set; }
    public int PreviousGlobalRank { get; set; }
    public int PreviousTopLevelDomainRank { get; set; }
    public int PreviousReferringSubNets { get; set; }
    public int PreviousReferringIps { get; set; }
}

public class SiteRankingSearch
{
    public SearchCriteria<int> GlobalRank { get; set; }
    public SearchCriteria<int> TopLevelDomainRank { get; set; }
    public SearchCriteria<string> DomainName { get; set; }
    public SearchCriteria<string> TopLevelDomain { get; set; }
}
```

2. Build the table by extending the class `Table<TRow, TSearch>`
    1. Configure the schema of your table by mapping every field
    2. Configure the indexes by enumerating every criteria

```csharp
using ZumtenSoft.Mindex;

public class SiteRankingTable : Table<SiteRanking, SiteRankingSearch>
{
    public SiteRankingTable(IReadOnlyCollection<SiteRanking> rankings) : base(rankings)
    {
        MapSearchCriteria(s => s.GlobalRank,         r => r.GlobalRank);
        MapSearchCriteria(s => s.TopLevelDomainRank, r => r.TopLevelDomainRank);
        MapSearchCriteria(s => s.DomainName,         r => r.DomainName,     StringComparer.OrdinalIgnoreCase);
        MapSearchCriteria(s => s.TopLevelDomain,     r => r.TopLevelDomain, StringComparer.OrdinalIgnoreCase);

        BuildIndex(s => s.TopLevelDomainRank);
        BuildIndex(s => s.TopLevelDomain, s => s.TopLevelDomainRank);
    }
}
```

3. Instanciate the table.
```csharp
List<SiteRanking> rankings = LoadRankings();
SiteRankingTable table = new SiteRankingTable(rankings);
```

4. Search the table.
```csharp
// Will use the index "TopLevelDomainRank"
// - TopLevelDomainRank: Extract 1 ArraySegment using BinarySearch
var top10OfEachTLD = table.Search(new SiteRankingSearch
{
    TopLevelDomainRank = SearchCriteria.ByRange(1, 10)
});

// Will use the index "TopLevelDomain_TopLevelDomainRank"
// - TopLevelDomain: Will extract 3 ArraySegments using BinarySearch
// - TopLevelDomainRank: For each ArraySegment, a new BinarySearch is applied
var top1000OfMainDomains = table.Search(new SiteRankingSearch
{
    TopLevelDomain = new [] { "com", "org", "net" },
    TopLevelDomainRank = SearchCriteria.ByRange(1, 1000)
});

// Will use the index "TopLevelDomain_TopLevelDomainRank":
// - TopLevelDomain: Will extract 1 ArraySegment using BinarySearch
// - GlobalRank: Filter the ArraySegment using a full scan
var canadianDomainsInTop1000 = table.Search(new SiteRankingSearch
{
    TopLevelDomain = "ca",
    GlobalRank = SearchCriteria.ByRange(1, 1000)
});
```

# Performance

The main point of this library is to provide easy search criterias without impacting the performance. You could always optimize a LookupTable for your specific needs and it will always be faster than Mindex. If you have to support multiple criterias, then this is when Mindex will shine because you can improve your performances with only one or two lines of code.

## Benchmarks

1. Top 10 domains for each top level domain ([SiteRankingSearchTopDomainByTLD.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/Benchmarks/SiteRankingSearchTopDomainByTLD.cs))

 Method |    N |        Mean |      Error |     StdDev |
------- |----- |------------:|-----------:|-----------:|
   Linq | 1000 | 26,075.3 us | 131.068 us | 122.601 us |
 Lookup | 1000 |    270.5 us |   1.723 us |   1.612 us |
 Search | 1000 |    163.5 us |   1.117 us |   1.045 us |


2. Top 1000 domains for each top level domain .com, .org and .net ([SiteRankingSearchTopDomainByComOrgNet.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/Benchmarks/SiteRankingSearchTopDomainByComOrgNet.cs))

 Method |    N |         Mean |       Error |      StdDev |
------- |----- |-------------:|------------:|------------:|
   Linq | 1000 | 59,399.82 us | 389.2275 us | 364.0836 us |
 Lookup | 1000 |     85.42 us |   0.6370 us |   0.5647 us |
 Search | 1000 |     75.47 us |   0.4397 us |   0.4113 us |


3. Canadian sites part of the top 1000 global websites ([SiteRankingSearchTopCanadianDomain.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/Benchmarks/SiteRankingSearchTopCanadianDomain.cs))

|                        Method |    N |            Mean |          Error |         StdDev |
|------------------------------ |----- |----------------:|---------------:|---------------:|
|                          Linq | 1000 | 28,240,356.7 ns | 263,440.000 ns | 246,421.916 ns |
|                        Lookup | 1000 |        509.0 ns |       3.115 ns |       2.432 ns |
|                        Search | 1000 |      3,788.4 ns |      29.451 ns |      27.548 ns |
|               IndexGlobalRank | 1000 |    187,289.7 ns |   1,286.063 ns |   1,202.984 ns |
|           IndexTopLevelDomain | 1000 |    506,596.8 ns |   4,577.985 ns |   4,282.250 ns |
| IndexTopLevelDomainGlobalRank | 1000 |      3,355.8 ns |      27.983 ns |      23.367 ns |