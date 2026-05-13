using Game.UI.Data;
using R3;

namespace Game.UI.Service
{
    public interface ICashPurchaseService
    {
        Observable<PurchaseResult> Purchase(string productId);
    }
}