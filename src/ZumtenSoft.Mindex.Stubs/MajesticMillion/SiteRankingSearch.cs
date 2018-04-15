using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Stubs.MajesticMillion
{
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

        public SearchCriteriaByValue<char> TLDContainsChar { get; set; }
    }
}