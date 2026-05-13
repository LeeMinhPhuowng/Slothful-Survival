namespace Game.UI.Data
{
    public readonly struct CurrencyChangedPayload
    {
        public readonly CurrencyType CurrencyType;
        public readonly int OldAmount;
        public readonly int NewAmount;
        public readonly int Delta;
        public readonly string Source;

        public CurrencyChangedPayload(CurrencyType currencyType, int oldAmount, int newAmount, string source = "")
        {
            CurrencyType = currencyType;
            OldAmount = oldAmount;
            NewAmount = newAmount;
            Delta = newAmount - oldAmount;
            Source = source;
        }
    }
}