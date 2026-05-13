namespace Game.UI.Data
{
    public readonly struct PlayerDiedPayload
    {
        public readonly string CharacterId;
        public readonly string CauseId;
        public readonly float RunTimeSeconds;

        public PlayerDiedPayload(string characterId, string causeId, float runTimeSeconds)
        {
            CharacterId = characterId;
            CauseId = causeId;
            RunTimeSeconds = runTimeSeconds;
        }
    }
}