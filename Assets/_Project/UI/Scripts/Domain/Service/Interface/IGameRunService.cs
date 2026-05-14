using Game.UI.Model;
using R3;

namespace Game.UI.Service
{
    public interface IGameRunService
    {
        ReadOnlyReactiveProperty<bool> IsPaused { get; }
        ReadOnlyReactiveProperty<RunResultModel> CurrentRunResult { get; }
        ReadOnlyReactiveProperty<int> ReviveRemainingSeconds { get; }
        ReadOnlyReactiveProperty<float> ReviveTimerPercent { get; }
        int ReviveGoldCost { get; }
        void Pause();
        void Resume();
        void TryAgain();
        void NextLevel();
        void ReturnToChoosingMap();
        void ReviveByGold();
    }
}
