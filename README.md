# mindex
Mindex: DotNet High-performance in-memory multi-criteria collection search

[![Build Status](https://zumten.visualstudio.com/_apis/public/build/definitions/d6fe51c2-2715-43c8-8bff-5cb5575470b4/1/badge)](https://zumten.visualstudio.com/ZumtenSoft/_build/index?definitionId=1)

## Objective

This library tries to mimmick the way you can search inside a table in SQL through multiple criterias, while providing the full performance of in-memory objects.

## How to install?

The library is currently only released through NuGet https://www.nuget.org/packages/ZumtenSoft.Mindex/


## How it works?

Mindex tries to mimmick the model of SQL indexes. An index is created by sorting the list of rows in a way that multiple criterias can be applied one by one to reduce the range of results (as long and the search matches the index). The search is then done using BinarySearch for each criteria. It is possible to force an index, but by default, Mindex analyzes the search, give a score to each index and chooses the best one.

For more information, refer to the class [BinarySearchResult](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex/BinarySearchResult.cs).


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
        MapCriteria(s => s.Date, i => i.Date);
        MapCriteria(s => s.Type, i => i.Type);
        MapCriteria(s => s.Origin, i => i.Origin);
        MapCriteria(s => s.Quantity, i => i.Quantity);
        MapCriteria(s => s.QuantityUnitCode, i => i.QuantityUnitCode);
        MapCriteria(s => s.QuantityType, i => i.QuantityType);
        MapCriteria(s => s.ImportState, i => i.ImportState);
        MapCriteria(s => s.ImportLocationCode, i => i.ImportLocationCode);
        MapCriteria(s => s.ImportLocationName, i => i.ImportLocationName);
        MapCriteria(s => s.TariffHeading, i => i.TariffHeading);
        MapCriteria(s => s.GoodsValue, i => i.GoodsValue);

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
    // You can specify multiple values by assigning an array
    Origin = new string[] {"CANADA", "UNITED STATES", "MEXICO"},
    QuantityType = new string[] {"W", "M"},

    // You can specify a single value by assigning the value matching the criteria type
    ImportState = "Delhi",

    // You can specify a range of values by calling SearchCriteria.ByRange
    Date = SearchCriteria.ByRange(new DateTime(2015, 5, 1), new DateTime(2015, 5, 14)),

    // You can also specify a custom predicate which will be filtered after everything else.
    // In this case, the compiler can't determine the type. You will have to specify it yourself.
    GoodsValue = SearchCriteria.ByPredicate((decimal v) => v > 0)
});
```

# Performance

The main point of this library is to provide easy search criterias without impacting the performance. You could always optimize a LookupTable for your specific needs and it will always be faster than Mindex. If you have to support multiple criterias, then this is when Mindex will shine because you can improve your performances with only one or two lines of code.

## Benchmarks

1. Simple search (1 criteria, single value) ([SearchBySingleOrigin.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/IndianCustoms/SearchBySingleOrigin.cs))

``` ini
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.14393.2189 (1607/AnniversaryUpdate/Redstone1)
Intel Xeon CPU E5-2673 v3 2.40GHz, 1 CPU, 2 logical cores and 1 physical core
.NET Core SDK=2.1.104
  [Host]     : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
```
|       Method |    N |       Mean |     Error |    StdDev |
|------------- |----- |-----------:|----------:|----------:|
|   SearchLinq | 1000 | 102.633 ms | 2.0321 ms | 3.5053 ms |
| SearchLookup | 1000 |   3.479 ms | 0.0692 ms | 0.0850 ms |
| SearchMindex | 1000 |   3.444 ms | 0.0685 ms | 0.0762 ms |




2. Simple search (1 criteria, multiple values) ([SearchByMultipleOrigins.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/IndianCustoms/SearchByMultipleOrigins.cs))

``` ini
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.14393.2189 (1607/AnniversaryUpdate/Redstone1)
Intel Xeon CPU E5-2673 v3 2.40GHz, 1 CPU, 2 logical cores and 1 physical core
.NET Core SDK=2.1.104
  [Host]     : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
```
|       Method |    N |       Mean |     Error |    StdDev |     Median |
|------------- |----- |-----------:|----------:|----------:|-----------:|
|   SearchLinq | 1000 | 400.799 ms | 7.9923 ms | 6.6739 ms | 401.103 ms |
| SearchLookup | 1000 |   7.684 ms | 0.2332 ms | 0.6876 ms |   8.045 ms |
| SearchMindex | 1000 |   3.915 ms | 0.0741 ms | 0.0693 ms |   3.908 ms |




3. Search with 4 criterias (3 by values, 1 by range) ([SearchByOriginDestinationQuantityTypeDate.cs](https://github.com/zumten/mindex/blob/master/src/ZumtenSoft.Mindex.Benchmark/IndianCustoms/SearchByOriginDestinationQuantityTypeDate.cs))

``` ini
BenchmarkDotNet=v0.10.14, OS=Windows 10.0.14393.2189 (1607/AnniversaryUpdate/Redstone1)
Intel Xeon CPU E5-2673 v3 2.40GHz, 1 CPU, 2 logical cores and 1 physical core
.NET Core SDK=2.1.104
  [Host]     : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.6 (CoreCLR 4.6.26212.01, CoreFX 4.6.26212.01), 64bit RyuJIT
```
|                            Method |    N |         Mean |        Error |       StdDev |
|---------------------------------- |----- |-------------:|-------------:|-------------:|
|                        SearchLinq | 1000 | 309,498.1 us | 2,104.052 us | 1,756.979 us |
|                      SearchLookup | 1000 |   2,777.5 us |    55.091 us |    75.409 us |
|      SearchLookupWithBinarySearch | 1000 |     589.0 us |     7.527 us |     6.673 us |
| SearchOrderedListWithBinarySearch | 1000 |     123.0 us |     2.389 us |     2.844 us |
|                      SearchMindex | 1000 |     176.2 us |     3.499 us |     5.128 us |

