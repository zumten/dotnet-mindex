using System.Diagnostics;

namespace ZumtenSoft.Mindex.Tests.Stubs
{
    [DebuggerDisplay(@"\{SiteRanking DomainName={DomainName}, GlobalRank={GlobalRank}, TopLevelDomainRank={TopLevelDomainRank} \}")]
    public class SiteRanking
    {
        public int GlobalRank { get; set; }
        public int TopLevelDomainRank { get; set; }
        public string DomainName { get; set; }
        public string TopLevelDomain { get; set; }
        public int ReferringSubNets { get; set; }
        public int ReferringIps { get; set; }
        public string InternationalizedDomainName { get; set; }
        public string InternationalizedTopLevelDomain { get; set; }
        public int PreviousGlobalRank { get; set; }
        public int PreviousTopLevelDomainRank { get; set; }
        public int PreviousReferringSubNets { get; set; }
        public int PreviousReferringIps { get; set; }
    }
}
