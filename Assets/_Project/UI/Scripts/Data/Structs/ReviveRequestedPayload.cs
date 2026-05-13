namespace Game.UI.Data
{
    public readonly struct ReviveRequestedPayload
    {
        public readonly string CharacterId;
        public readonly RevivePaymentType PaymentType;

        public ReviveRequestedPayload(string characterId, RevivePaymentType paymentType)
        {
            CharacterId = characterId;
            PaymentType = paymentType;
        }
    }
}