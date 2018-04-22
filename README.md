# mindex
Mindex: DotNet High-performance in-memory multi-criteria collection search

[![Build Status](https://zumten.visualstudio.com/_apis/public/build/definitions/d6fe51c2-2715-43c8-8bff-5cb5575470b4/1/badge)](https://zumten.visualstudio.com/ZumtenSoft/_build/index?definitionId=1)

## Objective

This library aims to optimize search over a read-only list using a large number of criterias. It is like a big lookup table that can support multiple type of search instead of just the one it was made for.

If you have one or many of the following requirements, Mindex can help you:
- Your application has to search over very big collections of data.
- Your application has collections with a bunch of different search criterias.
- Your application has dynamic business rules that cannot be preoptimized.


## How to install?

The library is currently only released through NuGet https://www.nuget.org/packages/ZumtenSoft.Mindex/


## How it works?

Mindex tries to mimmick the model of SQL indexes. An index is created by sorting the list of rows in a way that multiple criterias can be applied one by one to reduce the range of results (as long and the search matches the index). The search is then done using BinarySearch for each criteria.

For more information about how an index is built and searched, refer to the wiki page dedicated to [BinarySearchTable](https://github.com/zumten/mindex/wiki/BinarySearchTable).

Mindex adds a layer on top of ArraySegmentCollection by analyzing the search, giving a score to each index and choosing the best one. This way you can focus on your business requirements and not on how to build your data structure for performance.


## Build your first table

There are 5 simple steps to build your own table with indexes

1. Include the namespaces

```csharp
using ZumtenSoft.Mindex;
using ZumtenSoft.Mindex.Criterias;
```

2. Create your models:
    - Create your POCO object
    - Create a search object
        - Duplicate your class by appending the suffix "Search"
        - Remove the properties you won't need to search
        - Wrap the type of the properties within a SearchCriteria<T>

```csharp
// Example from the Indian Customs - List of all imports between 2015-05-01 to 2015-09-11
public class Import
{
    public int SerialId { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public string Origin { get; set; }

    public decimal Quantity { get; set; }
    public string QuantityUnitCode { get; set; }
    public string QuantityDescription { get; set; }
    public string QuantityType { get; set; }

    public string ImportState { get; set; }
    public string ImportLocationCode { get; set; }
    public string ImportLocationName { get; set; }

    public int TariffHeading { get; set; }
    public decimal GoodsValue { get; set; }
    public string GoodDescription { get; set; }
}

public class ImportSearch
{
    public SearchCriteria<DateTime> Date { get; set; }
    public SearchCriteria<string> Type { get; set; }
    public SearchCriteria<string> Origin { get; set; }

    public SearchCriteria<decimal> Quantity { get; set; }
    public SearchCriteria<string> QuantityUnitCode { get; set; }
    public SearchCriteria<string> QuantityType { get; set; }

    public SearchCriteria<string> ImportState { get; set; }
    public SearchCriteria<string> ImportLocationCode { get; set; }
    public SearchCriteria<string> ImportLocationName { get; set; }
    public SearchCriteria<int> TariffHeading { get; set; }
    public SearchCriteria<decimal> GoodsValue { get; set; }
}
```

3. Build the table by extending the class `Table<TRow, TSearch>`
    1. Configure the schema of your table by mapping every field
    2. Configure the indexes by enumerating every criteria

```csharp
using ZumtenSoft.Mindex;

public class ImportTable : Table<Import, ImportSearch>
{
    public ImportTable(IReadOnlyCollection<Import> rows) : base(rows)
    {
        MapCriteria(s => s.Date).ToProperty(i => i.Date);
        MapCriteria(s => s.Type).ToProperty(i => i.Type);
        MapCriteria(s => s.Origin).ToProperty(i => i.Origin);
        MapCriteria(s => s.Quantity).ToProperty(i => i.Quantity);
        MapCriteria(s => s.QuantityUnitCode).ToProperty(i => i.QuantityUnitCode);
        MapCriteria(s => s.QuantityType).ToProperty(i => i.QuantityType);
        MapCriteria(s => s.ImportState).ToProperty(i => i.ImportState);
        MapCriteria(s => s.ImportLocationCode).ToProperty(i => i.ImportLocationCode);
        MapCriteria(s => s.ImportLocationName).ToProperty(i => i.ImportLocationName);
        MapCriteria(s => s.TariffHeading).ToProperty(i => i.TariffHeading);
        MapCriteria(s => s.GoodsValue).ToProperty(i => i.GoodsValue);

        ConfigureIndex().IncludeColumns(s => s.Origin, s => s.ImportState, s => s.QuantityType, s => s.Date).Build();
    }
}
```

4. Instanciate the table.
```csharp
List<Import> rows = ... //LoadImports();
ImportTable table = new ImportTable(rows);
```
5. Search the table with as many criterias as you wish.
```csharp
Import[] result = table.Search(new ImportSearch
{
    
    // You can specify a single value criteria
    ImportState = "Delhi",

    // You can specify multiple values by assigning an array
    Origin = new string[] {"CANADA", "UNITED STATES", "MEXICO"},
    QuantityType = new string[] {"W", "M"},

    // You can specify a range of values by calling SearchCriteria.ByRange
    Date = SearchCriteria.ByRange(new DateTime(2015, 5, 1), new DateTime(2015, 5, 14)),

    // You can also specify a custom predicate which will be filtered after everything else.
    // In this case, the compiler can't determine the type. You will have to specify it yourself.
    GoodsValue = SearchCriteria.ByPredicate((decimal v) => v > 0)
});
```

# Performance

Mindex is optimized to search over a bunch of different criterias. The more complex are your criterias, the more mindex can help you optimize your search (as long as it can match an index).


## Benchmarks

1. Simple search (1 criteria, single value) ([SearchBySingleOrigin.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/IndianCustoms/SearchBySingleOrigin.cs))

```ini
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.14393.2189 (1607/AnniversaryUpdate/Redstone1)
Intel Xeon CPU E5-2673 v3 2.40GHz, 1 CPU, 2 logical cores and 1 physical core
.NET Core SDK=2.1.104
  [Host]                   : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  .NET Core 2 (x64)        : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  .NET Framework 4.7 (x64) : .NET Framework 4.7.1 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2558.0
  .NET Framework 4.7 (x86) : .NET Framework 4.7.1 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2558.0

Jit=RyuJit  
```
|      Method |                      Job |       Mean |    StdDev | Allocated |
|------------ |------------------------- |-----------:|----------:|----------:|
|  SearchLinq |        .NET Core 2 (x64) | 104.148 ms | 2.2608 ms |      8 MB |
|SearchLookup |        .NET Core 2 (x64) |   3.655 ms | 0.0644 ms |    2.8 MB |
|SearchMindex |        .NET Core 2 (x64) |   3.646 ms | 0.0650 ms |    2.8 MB |
|             |                          |            |           |           |
|  SearchLinq | .NET Framework 4.7 (x64) | 104.811 ms | 1.7615 ms |      8 MB |
|SearchLookup | .NET Framework 4.7 (x64) |   4.304 ms | 0.0722 ms |    2.8 MB |
|SearchMindex | .NET Framework 4.7 (x64) |   4.309 ms | 0.0510 ms |    2.8 MB |
|             |                          |            |           |           |
|  SearchLinq | .NET Framework 4.7 (x86) |  54.385 ms | 1.7233 ms |      4 MB |
|SearchLookup | .NET Framework 4.7 (x86) |   2.930 ms | 0.0560 ms |    1.4 MB |
|SearchMindex | .NET Framework 4.7 (x86) |   2.935 ms | 0.0439 ms |    1.4 MB |





2. Simple search (1 criteria, multiple values) ([SearchByMultipleOrigins.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/IndianCustoms/SearchByMultipleOrigins.cs))

```ini
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.14393.2189 (1607/AnniversaryUpdate/Redstone1)
Intel Xeon CPU E5-2673 v3 2.40GHz, 1 CPU, 2 logical cores and 1 physical core
.NET Core SDK=2.1.104
  [Host]                   : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  .NET Core 2 (x64)        : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  .NET Framework 4.7 (x64) : .NET Framework 4.7.1 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2558.0
  .NET Framework 4.7 (x86) : .NET Framework 4.7.1 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2558.0

Jit=RyuJit  
```
|       Method |                      Job |       Mean |    StdDev | Allocated |
|------------- |------------------------- |-----------:|----------:|----------:|
|   SearchLinq |        .NET Core 2 (x64) | 423.363 ms | 4.6193 ms |      8 MB |
| SearchLookup |        .NET Core 2 (x64) |   8.257 ms | 0.6570 ms |   8.69 MB |
| SearchMindex |        .NET Core 2 (x64) |   4.111 ms | 0.0653 ms |   3.13 MB |
|              |                          |            |           |           |
|   SearchLinq | .NET Framework 4.7 (x64) | 518.641 ms | 5.5392 ms |      8 MB |
| SearchLookup | .NET Framework 4.7 (x64) |  42.552 ms | 0.7375 ms |      8 MB |
| SearchMindex | .NET Framework 4.7 (x64) |   4.570 ms | 0.0789 ms |   3.13 MB |
|              |                          |            |           |           |
|   SearchLinq | .NET Framework 4.7 (x86) | 422.906 ms | 4.8710 ms |      4 MB |
| SearchLookup | .NET Framework 4.7 (x86) |  30.837 ms | 0.5665 ms |      4 MB |
| SearchMindex | .NET Framework 4.7 (x86) |   3.078 ms | 0.0545 ms |   1.56 MB |





3. Search with 4 criterias (3 by values, 1 by range) ([SearchByOriginDestinationQuantityTypeDate.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/IndianCustoms/SearchByOriginDestinationQuantityTypeDate.cs))

```ini
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.14393.2189 (1607/AnniversaryUpdate/Redstone1)
Intel Xeon CPU E5-2673 v3 2.40GHz, 1 CPU, 2 logical cores and 1 physical core
.NET Core SDK=2.1.104
  [Host]                   : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  .NET Core 2 (x64)        : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  .NET Framework 4.7 (x64) : .NET Framework 4.7.1 (CLR 4.0.30319.42000), 64bit RyuJIT-v4.7.2558.0
  .NET Framework 4.7 (x86) : .NET Framework 4.7.1 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2558.0

Jit=RyuJit  
```
|                            Method |                      Job |         Mean |       StdDev | Allocated |
|---------------------------------- |------------------------- |-------------:|-------------:|----------:|
|                        SearchLinq |        .NET Core 2 (x64) | 337,605.0 us | 7,040.079 us | 512.38 KB |
|                      SearchLookup |        .NET Core 2 (x64) |   3,027.7 us |   100.633 us | 512.78 KB |
|      SearchLookupWithBinarySearch |        .NET Core 2 (x64) |     116.5 us |     2.912 us | 171.74 KB |
| SearchOrderedListWithBinarySearch |        .NET Core 2 (x64) |     130.6 us |     3.305 us | 172.42 KB |
|                      SearchMindex |        .NET Core 2 (x64) |     186.2 us |     3.947 us | 175.23 KB |
|                                   |                          |              |              |           |
|                        SearchLinq | .NET Framework 4.7 (x64) | 567,135.3 us | 5,000.461 us | 512.73 KB |
|                      SearchLookup | .NET Framework 4.7 (x64) |   2,859.2 us |   103.706 us | 512.85 KB |
|      SearchLookupWithBinarySearch | .NET Framework 4.7 (x64) |     191.4 us |    31.063 us | 171.99 KB |
| SearchOrderedListWithBinarySearch | .NET Framework 4.7 (x64) |     163.1 us |     1.463 us | 172.42 KB |
|                      SearchMindex | .NET Framework 4.7 (x64) |     187.8 us |     3.968 us | 175.38 KB |
|                                   |                          |              |              |           |
|                        SearchLinq | .NET Framework 4.7 (x86) | 436,443.8 us | 9,130.098 us | 256.61 KB |
|                      SearchLookup | .NET Framework 4.7 (x86) |   2,894.3 us |    48.613 us | 256.52 KB |
|      SearchLookupWithBinarySearch | .NET Framework 4.7 (x86) |     131.3 us |     1.976 us |  86.15 KB |
| SearchOrderedListWithBinarySearch | .NET Framework 4.7 (x86) |     126.0 us |     1.546 us |  86.43 KB |
|                      SearchMindex | .NET Framework 4.7 (x86) |     141.0 us |     1.780 us |   88.1 KB |


