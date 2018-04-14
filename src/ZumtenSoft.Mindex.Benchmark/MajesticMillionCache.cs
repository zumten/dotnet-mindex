using System.Collections.Generic;
using System.Linq;
using ZumtenSoft.Mindex.Stubs.MajesticMillion;

namespace ZumtenSoft.Mindex.Benchmark
{
    public static class MajesticMillionCache
    {
        private static List<SiteRanking> _instance;
        
        public static List<SiteRanking> Instance => _instance ?? (_instance = MajesticMillionHelper.LoadSiteRankings(@"App_Data\majestic_million.csv").ToList());
    }
}
