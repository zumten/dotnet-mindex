using System;
using System.Collections.Generic;

namespace ZumtenSoft.Mindex.Stubs.StackOverflow
{
    public class StackOverflowUserTable : Table<StackOverflowUser, StackOverflowUserSearch>
    {
        public StackOverflowUserTable(IReadOnlyCollection<StackOverflowUser> rows) : base(rows)
        {
            MapCriteria(s => s.CreationDate, u => u.CreationDate);
            MapCriteria(s => s.LastAccessDate, u => u.LastAccessDate);
            MapCriteria(s => s.DisplayName, u => u.DisplayName, StringComparer.OrdinalIgnoreCase);
            MapCriteria(s => s.Location, u => u.Location, StringComparer.OrdinalIgnoreCase);
            MapCriteria(s => s.Views, u => u.Views);
            MapCriteria(s => s.UpVotes, u => u.UpVotes);
            MapCriteria(s => s.DownVotes, u => u.DownVotes);
            MapCriteria(s => s.RatioVotes, u => u.UpVotes / (float) (u.UpVotes + u.DownVotes));
            MapCriteria(s => s.Reputation, u => u.Reputation);

            ConfigureIndex().IncludeColumns(s => s.Location, s => s.Reputation).Build();
            ConfigureIndex().IncludeColumns(s => s.LastAccessDate).Build();
        }
    }
}
