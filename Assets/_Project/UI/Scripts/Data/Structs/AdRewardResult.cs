namespace Game.UI.Data
{
    public readonly struct AdRewardResult
    {
        public readonly bool Success;
        public readonly string PlacementId;
        public readonly RewardPayload Reward;
        public readonly string Message;

        public AdRewardResult(bool success, string placementId, RewardPayload reward, string message = "")
        {
            Success = success;
            PlacementId = placementId;
            Reward = reward;
            Message = message;
        }
    }
}