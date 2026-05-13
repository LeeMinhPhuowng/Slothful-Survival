using Game.UI.Data;
using R3;

namespace Game.UI.Service
{
    public interface IReviveService
    {
        ReadOnlyReactiveProperty<int> RemainingSeconds { get; }
        ReadOnlyReactiveProperty<float> TimerPercent { get; }
        ReadOnlyReactiveProperty<bool> CanReviveByAds { get; }
        ReadOnlyReactiveProperty<bool> CanReviveByDiamond { get; }
        Observable<ReviveResult> ReviveByAds();
        ReviveResult ReviveByDiamond();
    }
}