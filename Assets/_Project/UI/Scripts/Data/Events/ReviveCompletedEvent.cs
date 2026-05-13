namespace Game.UI.Data
{
    public readonly struct ReviveCompletedEvent
    {
        public readonly ReviveCompletedPayload Payload;

        public ReviveCompletedEvent(ReviveCompletedPayload payload)
        {
            Payload = payload;
        }
    }
}
