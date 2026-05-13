namespace Game.UI.Data
{
    public readonly struct ReviveResult
    {
        public readonly bool Success;
        public readonly ReviveFailureReason FailureReason;
        public readonly string Message;

        public ReviveResult(bool success, ReviveFailureReason failureReason, string message = "")
        {
            Success = success;
            FailureReason = failureReason;
            Message = message;
        }

        public static ReviveResult Succeeded(string message = "")
        {
            return new ReviveResult(true, ReviveFailureReason.None, message);
        }

        public static ReviveResult Failed(ReviveFailureReason failureReason, string message = "")
        {
            return new ReviveResult(false, failureReason, message);
        }
    }
}