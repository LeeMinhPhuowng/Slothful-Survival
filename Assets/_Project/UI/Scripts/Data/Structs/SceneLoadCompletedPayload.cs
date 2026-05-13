namespace Game.UI.Data
{
    public readonly struct SceneLoadCompletedPayload
    {
        public readonly SceneId LoadedScene;
        public readonly string SceneName;

        public SceneLoadCompletedPayload(SceneId loadedScene, string sceneName)
        {
            LoadedScene = loadedScene;
            SceneName = sceneName;
        }
    }
}