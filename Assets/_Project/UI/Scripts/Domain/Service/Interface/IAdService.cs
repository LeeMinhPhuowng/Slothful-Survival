using Game.UI.Data;
using R3;

namespace Game.UI.Service
{
    public interface IAdService
    {
        ReadOnlyReactiveProperty<bool> IsRewardedAdReady { get; }
        Observable<AdRewardResult> ShowRewardedAd(string placementId);
    }
}