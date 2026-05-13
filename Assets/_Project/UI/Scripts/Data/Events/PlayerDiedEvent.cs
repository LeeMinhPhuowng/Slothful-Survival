namespace Game.UI.Data
{
    public readonly struct PlayerDiedEvent
    {
        public readonly PlayerDiedPayload Payload;

        public PlayerDiedEvent(PlayerDiedPayload payload)
        {
            Payload = payload;
        }
    }
}
