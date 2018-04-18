namespace ZumtenSoft.Mindex.MappingCriterias
{
    public struct TableCriteriaScore
    {
        public static readonly TableCriteriaScore Initial = new TableCriteriaScore(1, true);
        public static readonly TableCriteriaScore Impossible = new TableCriteriaScore(0, false);
        public static readonly TableCriteriaScore NotOptimizable = new TableCriteriaScore(1, false);

        public TableCriteriaScore(float value, bool canContinue)
        {
            Value = value;
            CanContinue = canContinue;
        }

        public float Value { get; set; }
        public bool CanContinue { get; set; }

        public static TableCriteriaScore operator *(TableCriteriaScore score1, TableCriteriaScore score2)
        {
            return new TableCriteriaScore(score1.Value * score2.Value, score1.CanContinue && score2.CanContinue);
        }
    }
}
