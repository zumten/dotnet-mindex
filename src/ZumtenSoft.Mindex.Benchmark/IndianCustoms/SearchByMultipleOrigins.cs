using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using ZumtenSoft.Mindex.Criterias;
using ZumtenSoft.Mindex.Stubs.IndianCustoms;

namespace ZumtenSoft.Mindex.Benchmark.IndianCustoms
{
    public class SearchByMultipleOrigins : IndianCustomsImportBenchmark
    {
        private static readonly string[] Origins = {"CANADA", "UNITED STATES", "MEXICO"};

        public ILookup<string, CustomsImport> LookupTable { get; set; }
        public override void Setup()
        {
            base.Setup();
            LookupTable = Imports.ToLookup(i => i.Origin);
        }

        [Benchmark]
        public List<CustomsImport> SearchLinq() => Imports
            .Where(i => Origins.Contains(i.Origin))
            .ToList();

        [Benchmark]
        public List<CustomsImport> SearchLookup() =>
            Origins
                .SelectMany(x => LookupTable[x])
                .ToList();

        [Benchmark]
        public CustomsImport[] SearchMindex() => Table.Search(new CustomsImportSearch
        {
            Origin = Origins
        });
    }
}