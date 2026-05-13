namespace Game.UI.Data
{
    public readonly struct SceneLoadProgressPayload
    {
        public readonly SceneId TargetScene;
        public readonly float Progress;

        public SceneLoadProgressPayload(SceneId targetScene, float progress)
        {
            TargetScene = targetScene;
            Progress = progress;
        }
    }
}