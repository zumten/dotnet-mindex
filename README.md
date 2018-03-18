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

``` ini
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical cores and 4 physical cores
Frequency=1945312 Hz, Resolution=514.0564 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (CoreCLR 4.6.25815.02, CoreFX 4.6.25814.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.0.3 (CoreCLR 4.6.25815.02, CoreFX 4.6.25814.01), 64bit RyuJIT
```
| Method |    N |        Mean |      Error |     StdDev |
|------- |----- |------------:|-----------:|-----------:|
|   Linq | 1000 | 27,658.2 us | 477.536 us | 446.687 us |
| Lookup | 1000 |    370.9 us |   7.208 us |   9.867 us |
| Search | 1000 |    185.3 us |   3.533 us |   3.927 us |



2. Top 1000 domains for each top level domain .com, .org and .net ([SiteRankingSearchTopDomainByComOrgNet.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/Benchmarks/SiteRankingSearchTopDomainByComOrgNet.cs))

``` ini
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical cores and 4 physical cores
Frequency=1945312 Hz, Resolution=514.0564 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (CoreCLR 4.6.25815.02, CoreFX 4.6.25814.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.0.3 (CoreCLR 4.6.25815.02, CoreFX 4.6.25814.01), 64bit RyuJIT
```
| Method |    N |         Mean |      Error |      StdDev |
|------- |----- |-------------:|-----------:|------------:|
|   Linq | 1000 | 84,992.30 us | 853.119 us | 712.3928 us |
| Lookup | 1000 |     68.78 us |   1.274 us |   0.9946 us |
| Search | 1000 |     60.63 us |   1.199 us |   2.0996 us |



3. Canadian sites part of the top 1000 global websites ([SiteRankingSearchTopCanadianDomain.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/Benchmarks/SiteRankingSearchTopCanadianDomain.cs))

``` ini
BenchmarkDotNet=v0.10.13, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.309)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical cores and 4 physical cores
Frequency=1945312 Hz, Resolution=514.0564 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (CoreCLR 4.6.25815.02, CoreFX 4.6.25814.01), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.0.3 (CoreCLR 4.6.25815.02, CoreFX 4.6.25814.01), 64bit RyuJIT
```
|                        Method |    N |            Mean |         Error |        StdDev |
|------------------------------ |----- |----------------:|--------------:|--------------:|
|                          Linq | 1000 | 29,218,023.0 ns | 213,604.01 ns | 199,805.30 ns |
|                        Lookup | 1000 |        705.2 ns |      14.96 ns |      27.36 ns |
|                        Search | 1000 |      4,038.0 ns |      80.33 ns |     160.43 ns |
|               IndexGlobalRank | 1000 |    367,965.2 ns |   6,460.89 ns |   6,043.52 ns |
|           IndexTopLevelDomain | 1000 |    763,976.0 ns |  18,818.39 ns |  15,714.21 ns |
| IndexTopLevelDomainGlobalRank | 1000 |      2,522.2 ns |      50.26 ns |      82.57 ns |