namespace Game.UI.Data
{
    public readonly struct SceneLoadFailedPayload
    {
        public readonly SceneId TargetScene;
        public readonly SceneLoadFailureReason Reason;
        public readonly string Message;

        public SceneLoadFailedPayload(SceneId targetScene, SceneLoadFailureReason reason, string message = "")
        {
            TargetScene = targetScene;
            Reason = reason;
            Message = message;
        }
    }
}