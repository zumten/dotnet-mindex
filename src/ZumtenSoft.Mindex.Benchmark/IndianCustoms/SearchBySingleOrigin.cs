using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Stubs.IndianCustoms;

namespace ZumtenSoft.Mindex.Benchmark.IndianCustoms
{
    public class SearchBySingleOrigin : IndianCustomsImportBenchmark
    {
        private static readonly string Origin = "UNITED STATES";

        public ILookup<string, CustomsImport> LookupTable { get; set; }
        public override void Setup()
        {
            base.Setup();
            LookupTable = Imports.ToLookup(i => i.Origin);
        }

        [Benchmark]
        public List<CustomsImport> SearchLinq() => Imports
            .Where(i => Origin == i.Origin)
            .ToList();

        [Benchmark]
        public List<CustomsImport> SearchLookup() =>
            LookupTable[Origin].ToList();

        [Benchmark]
        public CustomsImport[] SearchMindex() => Table.Search(new CustomsImportSearch
        {
            Origin = Origin
        });
    }
}