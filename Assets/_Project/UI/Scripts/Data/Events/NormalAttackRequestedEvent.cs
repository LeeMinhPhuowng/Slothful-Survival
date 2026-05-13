namespace Game.UI.Data
{
    public readonly struct NormalAttackRequestedEvent
    {
        public readonly NormalAttackRequestedPayload Payload;

        public NormalAttackRequestedEvent(NormalAttackRequestedPayload payload)
        {
            Payload = payload;
        }
    }
}
