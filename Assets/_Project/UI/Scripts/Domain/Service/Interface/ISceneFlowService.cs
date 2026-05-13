using System.Threading;
using Cysharp.Threading.Tasks;
using Game.UI.Data;
using R3;

namespace Game.UI.Service
{
    public interface ISceneFlowService
    {
        SceneId CurrentScene { get; }
        bool IsLoading { get; }
        float LoadingProgress { get; }

        GameplayLoadRequest? LastGameplayLoadRequest { get; }

        UniTask<SceneLoadResult> LoadMainMenuAsync(CancellationToken cancellationToken = default);
        UniTask<SceneLoadResult> LoadChoosingMapAsync(CancellationToken cancellationToken = default);
        UniTask<SceneLoadResult> LoadGameplayAsync(GameplayLoadRequest request, CancellationToken cancellationToken = default);
        UniTask<SceneLoadResult> ReloadGameplayAsync(CancellationToken cancellationToken = default);
    }
}
