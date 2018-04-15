using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ZumtenSoft.Mindex.Stubs.MajesticMillion
{
    public static class MajesticMillionHelper
    {
        public static SiteRanking[] LoadSiteRankings()
        {
            return FileHelper.LoadFileWithCache("majestic_million.csv", f => LoadSiteRankingsFromCsv(f).ToArray());
        }

        public static IEnumerable<SiteRanking> LoadSiteRankingsFromCsv(string fileName)
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
                            DomainName = String.Intern(parts[2]),
                            TopLevelDomain = String.Intern(parts[3]),
                            ReferringSubNets = Int32.Parse(parts[4]),
                            ReferringIps = Int32.Parse(parts[5]),
                            InternationalizedDomainName = String.Intern(parts[6]),
                            InternationalizedTopLevelDomain = String.Intern(parts[7]),
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
