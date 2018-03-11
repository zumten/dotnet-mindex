# mindex
Mindex: C# Library to search through big in-memory collections

## Objective

This library tries to mimmick the way you can search inside a table in SQL through multiple criterias, while providing the full performance of in-memory objects.


## How it works?

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

2. Build the table by extending the class Table
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
var top10OfEachTLD = table.Search(new SiteRankingSearch
{
    TopLevelDomainRank = SearchCriteria.ByRange(1, 10)
});

// Will use the index "TopLevelDomain_TopLevelDomainRank"
var top1000OfMainDomains = table.Search(new SiteRankingSearch
{
    TopLevelDomain = new [] { "com", "org", "net" },
    TopLevelDomainRank = SearchCriteria.ByRange(1, 1000)
});
```
