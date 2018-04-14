using System;
using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes.Jobs;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Tests.Stubs.IndianCustoms;

namespace ZumtenSoft.Mindex.Benchmark.Benchmarks
{
    public class IndianCustomsImportSearchByOriginDestinationTypeDate : IndianCustomsImportBenchmark
    {
        private static readonly string[] Origins = {"CANADA", "UNITED STATES", "MEXICO"};
        private static readonly string[] Destinations = {"Delhi"};
        private static readonly string[] Sizes = {"W", "M"};
        private static readonly DateTime MinimumDate = new DateTime(2015, 5, 1);
        private static readonly DateTime MaximumDate = new DateTime(2015, 5, 14);

        private readonly Tuple<string, string, string>[] _lookupSearchCriterias =
            (from origin in Origins
                from destination in Destinations
                from size in Sizes
                select Tuple.Create(origin, destination, size)).ToArray();

        [Benchmark]
        public List<CustomsImport> Linq() => Imports
            .Where(i => Origins.Contains(i.Origin) && Destinations.Contains(i.ImportState) && Sizes.Contains(i.QuantityType) && i.Date >= MinimumDate && i.Date <= MaximumDate)
            .ToList();

        [Benchmark]
        public List<CustomsImport> Lookup() =>
            _lookupSearchCriterias
                .SelectMany(x => LookupTable[x])
                .Where(x => x.Date >= MinimumDate && x.Date <= MaximumDate)
                .ToList();


        [Benchmark]
        public List<CustomsImport> LookupWithBinarySearch() =>
            new BinarySearchResult<CustomsImport>(_lookupSearchCriterias.Select(x => LookupWithBinarySearchTable.TryGetValue(x, out var grp) ? new ArraySegment<CustomsImport>(grp) : ArraySegment<CustomsImport>.Empty).ToArray(), true)
                .ReduceRange(i => i.Date, MinimumDate, MaximumDate, Comparer<DateTime>.Default)
                .ToList();

        [Benchmark]
        public CustomsImport[] Search() => Table.Search(new CustomsImportSearch
        {
            Origin = Origins,
            ImportState = Destinations,
            QuantityType = Sizes,
            Date = SearchCriteria.ByRange(MinimumDate, MaximumDate)
        });
    }
}