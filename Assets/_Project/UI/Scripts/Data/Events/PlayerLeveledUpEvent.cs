namespace Game.UI.Data
{
    public readonly struct PlayerLeveledUpEvent
    {
        public readonly PlayerLeveledUpPayload Payload;

        public PlayerLeveledUpEvent(PlayerLeveledUpPayload payload)
        {
            Payload = payload;
        }
    }
}
