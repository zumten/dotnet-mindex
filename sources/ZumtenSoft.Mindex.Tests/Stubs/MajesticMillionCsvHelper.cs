using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZumtenSoft.Mindex.Tests.Stubs
{
    public static class MajesticMillionCache
    {
        private static List<SiteRanking> _instance;

        public static List<SiteRanking> Instance => _instance ?? (_instance = MajesticMillionCache.LoadSiteRankings(@"..\..\..\ZumtenSoft.Mindex.Tests\majestic_million.csv").ToList());

        public static IEnumerable<SiteRanking> LoadSiteRankings(string fileName)
        {
            using (FileStream file = File.OpenRead(fileName))
            using (StreamReader reader = new StreamReader(file))
            {
                if (reader.ReadLine() != null)
                {
                    for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    {
                        string[] parts = line.Split(',');
                        yield return new SiteRanking
                        {
                            GlobalRank = Int32.Parse(parts[0]),
                            TopLevelDomainRank = Int32.Parse(parts[1]),
                            DomainName = parts[2],
                            TopLevelDomain = parts[3],
                            ReferringSubNets = Int32.Parse(parts[4]),
                            ReferringIps = Int32.Parse(parts[5]),
                            InternationalizedDomainName = parts[6],
                            InternationalizedTopLevelDomain = parts[7],
                            PreviousGlobalRank = Int32.Parse(parts[8]),
                            PreviousTopLevelDomainRank = Int32.Parse(parts[9]),
                            PreviousReferringSubNets = Int32.Parse(parts[10]),
                            PreviousReferringIps = Int32.Parse(parts[11])
                        };
                    }
                }
            }
        }
    }
}
