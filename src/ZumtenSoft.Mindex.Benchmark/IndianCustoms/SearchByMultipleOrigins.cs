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

        public ILookup<string, Import> LookupTable { get; set; }
        public override void Setup()
        {
            base.Setup();
            LookupTable = Imports.ToLookup(i => i.Origin);
        }

        [Benchmark(Baseline = true)]
        public List<Import> SearchLinq() => Imports
            .Where(i => Origins.Contains(i.Origin))
            .ToList();

        [Benchmark]
        public List<Import> SearchLookup() =>
            Origins
                .SelectMany(x => LookupTable[x])
                .ToList();

        [Benchmark]
        public Import[] SearchMindex() => Table.Search(new ImportSearch
        {
            Origin = Origins
        });
    }
}