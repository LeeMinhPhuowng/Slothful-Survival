namespace Game.UI.Data
{
    public readonly struct SceneLoadStartedPayload
    {
        public readonly SceneId TargetScene;
        public readonly string SceneName;

        public SceneLoadStartedPayload(SceneId targetScene, string sceneName)
        {
            TargetScene = targetScene;
            SceneName = sceneName;
        }
    }
}