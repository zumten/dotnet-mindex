using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Tests.Stubs
{
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

    public class SiteRankingSearch
    {
        public SearchCriteria<int> GlobalRank { get; set; }
        public SearchCriteria<int> TopLevelDomainRank { get; set; }
        public SearchCriteria<string> DomainName { get; set; }
        public SearchCriteria<string> TopLevelDomain { get; set; }
        public SearchCriteria<int> ReferringSubNets { get; set; }
        public SearchCriteria<int> ReferringIps { get; set; }
        public SearchCriteria<string> InternationalizedDomainName { get; set; }
        public SearchCriteria<string> InternationalizedTopLevelDomain { get; set; }
    }
}
