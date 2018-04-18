using System;
using System.Collections.Generic;

namespace ZumtenSoft.Mindex.Stubs.StackOverflow
{
    public class StackOverflowUserTable : Table<StackOverflowUser, StackOverflowUserSearch>
    {
        public StackOverflowUserTable(IReadOnlyCollection<StackOverflowUser> rows) : base(rows)
        {
            MapCriteria(s => s.CreationDate).ToProperty(u => u.CreationDate);
            MapCriteria(s => s.LastAccessDate).ToProperty(u => u.LastAccessDate);
            MapCriteria(s => s.DisplayName).ToProperty(u => u.DisplayName, StringComparer.OrdinalIgnoreCase);
            MapCriteria(s => s.Location).ToProperty(u => u.Location, StringComparer.OrdinalIgnoreCase);
            MapCriteria(s => s.Views).ToProperty(u => u.Views);
            MapCriteria(s => s.UpVotes).ToProperty(u => u.UpVotes);
            MapCriteria(s => s.DownVotes).ToProperty(u => u.DownVotes);
            MapCriteria(s => s.RatioVotes).ToProperty(u => u.UpVotes / (float) (u.UpVotes + u.DownVotes));
            MapCriteria(s => s.Reputation).ToProperty(u => u.Reputation);

            ConfigureIndex().IncludeColumns(s => s.Location, s => s.Reputation).Build();
            ConfigureIndex().IncludeColumns(s => s.LastAccessDate).Build();
        }
    }
}
