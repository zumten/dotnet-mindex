using System.Diagnostics;

namespace ZumtenSoft.Mindex.Indexes
{
    [DebuggerDisplay(@"\{TableIndexScore Index={Index.Name}, Score={Score} \}")]
    public class TableIndexScore<TRow, TSearch>
    {
        public TableIndexScore(TableIndex<TRow, TSearch> index, float score)
        {
            Index = index;
            Score = score;
        }

        public TableIndex<TRow, TSearch> Index { get; }
        public float Score { get; }
    }
}