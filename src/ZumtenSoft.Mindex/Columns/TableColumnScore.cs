namespace ZumtenSoft.Mindex.Columns
{
    public struct TableColumnScore
    {
        public static readonly TableColumnScore Initial = new TableColumnScore(1, true);
        public static readonly TableColumnScore Impossible = new TableColumnScore(0, false);
        public static readonly TableColumnScore NotOptimizable = new TableColumnScore(1, false);

        public TableColumnScore(float value, bool canContinue)
        {
            Value = value;
            CanContinue = canContinue;
        }

        public float Value { get; set; }
        public bool CanContinue { get; set; }

        public static TableColumnScore operator *(TableColumnScore score1, TableColumnScore score2)
        {
            return new TableColumnScore(score1.Value * score2.Value, score1.CanContinue && score2.CanContinue);
        }
    }
}
