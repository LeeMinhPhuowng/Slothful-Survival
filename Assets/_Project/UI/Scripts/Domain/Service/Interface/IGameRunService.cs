using Game.UI.Model;
using R3;

namespace Game.UI.Service
{
    public interface IGameRunService
    {
        ReadOnlyReactiveProperty<bool> IsPaused { get; }
        ReadOnlyReactiveProperty<RunResultModel> CurrentRunResult { get; }
        void Pause();
        void Resume();
        void TryAgain();
        void ReturnToChoosingMap();
    }
}
