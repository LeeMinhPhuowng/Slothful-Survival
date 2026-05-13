namespace Game.UI.Data
{
    public readonly struct RewardGrantedEvent
    {
        public readonly RewardPayload Reward;
        public readonly string Source;

        public RewardGrantedEvent(RewardPayload reward, string source = "")
        {
            Reward = reward;
            Source = source;
        }
    }
}
