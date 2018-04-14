using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Stubs.IndianCustoms;

namespace ZumtenSoft.Mindex.Benchmark
{
    public static class CustomsImportsCache
    {
        private static List<CustomsImport> _instance;
        
        public static List<CustomsImport> Instance => _instance ?? (_instance = CustomsImportHelper.LoadCustomsImports(@"App_Data\IndianCustoms-Imports.csv").ToList());
    }
}
