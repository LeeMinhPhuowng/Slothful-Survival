using Game.UI.Data;

namespace Game.UI.Core
{
    public interface ISceneNameRegistry
    {
        bool TryGetSceneName(SceneId sceneId, out string sceneName);
    }
}