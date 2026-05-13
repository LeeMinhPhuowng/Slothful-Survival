namespace Game.UI.Data
{
    public readonly struct SceneLoadResult
    {
        public readonly bool Success;
        public readonly SceneLoadFailureReason FailureReason;
        public readonly SceneId TargetScene;
        public readonly string Message;

        public SceneLoadResult(bool success, SceneLoadFailureReason failureReason, SceneId targetScene, string message = "")
        {
            Success = success;
            FailureReason = failureReason;
            TargetScene = targetScene;
            Message = message;
        }

        public static SceneLoadResult Succeeded(SceneId targetScene, string message = "")
        {
            return new SceneLoadResult(true, SceneLoadFailureReason.None, targetScene, message);
        }

        public static SceneLoadResult Failed(SceneId targetScene, SceneLoadFailureReason failureReason, string message = "")
        {
            return new SceneLoadResult(false, failureReason, targetScene, message);
        }
    }
}