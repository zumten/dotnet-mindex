using System.Collections.Generic;
using System.Linq;

namespace ZumtenSoft.Mindex.Stubs.IndianCustoms
{
    public static class CustomsImportsCache
    {
        private static List<CustomsImport> _instance;
        
        public static List<CustomsImport> Instance => _instance ?? (_instance = CustomsImportHelper.LoadCustomImports(FileHelper.FindDirectory("App_Data") + @"\IndianCustoms-Imports.csv").ToList());
    }
}
