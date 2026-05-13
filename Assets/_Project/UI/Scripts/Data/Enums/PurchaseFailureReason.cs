namespace Game.UI.Data
{
    public enum PurchaseFailureReason
    {
        None,
        OfferNotFound,
        ProductUnavailable,
        InsufficientGold,
        InsufficientDiamond,
        InventoryFull,
        AlreadyOwned,
        PaymentCancelled,
        PaymentFailed,
        NetworkError,
        Unknown
    }
}