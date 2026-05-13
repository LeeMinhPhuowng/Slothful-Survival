namespace Game.UI.Data
{
    public readonly struct CurrencyRewardPayload
    {
        public readonly CurrencyType CurrencyType;
        public readonly int Amount;

        public CurrencyRewardPayload(CurrencyType currencyType, int amount)
        {
            CurrencyType = currencyType;
            Amount = amount;
        }
    }
}