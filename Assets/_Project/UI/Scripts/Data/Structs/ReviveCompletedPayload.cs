namespace Game.UI.Data
{
    public readonly struct ReviveCompletedPayload
    {
        public readonly string CharacterId;
        public readonly bool Success;
        public readonly ReviveFailureReason FailureReason;

        public ReviveCompletedPayload(string characterId, bool success, ReviveFailureReason failureReason)
        {
            CharacterId = characterId;
            Success = success;
            FailureReason = failureReason;
        }
    }
}