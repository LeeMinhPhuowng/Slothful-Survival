namespace Game.UI.Data
{
    public readonly struct PurchaseCompletedEvent
    {
        public readonly string OfferId;
        public readonly PurchaseResult Result;

        public PurchaseCompletedEvent(string offerId, PurchaseResult result)
        {
            OfferId = offerId;
            Result = result;
        }
    }
}
