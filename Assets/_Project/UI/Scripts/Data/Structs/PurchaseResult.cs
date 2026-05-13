namespace Game.UI.Data
{
    public readonly struct PurchaseResult
    {
        public readonly bool Success;
        public readonly PurchaseFailureReason FailureReason;
        public readonly RewardPayload Reward;
        public readonly string Message;

        public PurchaseResult(bool success, PurchaseFailureReason failureReason, RewardPayload reward, string message = "")
        {
            Success = success;
            FailureReason = failureReason;
            Reward = reward;
            Message = message;
        }

        public static PurchaseResult Succeeded(RewardPayload reward, string message = "")
        {
            return new PurchaseResult(true, PurchaseFailureReason.None, reward, message);
        }

        public static PurchaseResult Failed(PurchaseFailureReason failureReason, string message = "")
        {
            return new PurchaseResult(false, failureReason, RewardPayload.Empty, message);
        }
    }
}