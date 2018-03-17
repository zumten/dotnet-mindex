namespace ZumtenSoft.Mindex.Columns
{
    public class TableColumnScore
    {
        public TableColumnScore(float value, bool canContinue)
        {
            Value = value;
            CanContinue = canContinue;
        }

        public float Value { get; set; }
        public bool CanContinue { get; set; }
    }
}
