namespace Core.Foundation.Enums
{

    /// <summary>
    /// Types of currencies in the game.
    /// </summary>
    public enum CurrencyType
    {
        None = 0,
        Energy = 1,     // Time-based regenerating currency
        Gold = 2,       // Soft currency (gameplay rewards)
        Gem = 3         // Hard currency (IAP/achievements)
    }
}
