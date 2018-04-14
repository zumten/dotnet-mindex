using System;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Tests.Stubs.StackOverflow
{
    public class StackOverflowUserSearch
    {
        public SearchCriteria<int> Reputation { get; set; }
        public SearchCriteria<DateTime> CreationDate { get; set; }
        public SearchCriteria<string> DisplayName { get; set; }
        public SearchCriteria<DateTime> LastAccessDate { get; set; }
        public SearchCriteria<string> Location { get; set; }
        public SearchCriteria<int> Views { get; set; }
        public SearchCriteria<int> UpVotes { get; set; }
        public SearchCriteria<int> DownVotes { get; set; }
        public SearchCriteria<float> RatioVotes { get; set; }
    }
}
