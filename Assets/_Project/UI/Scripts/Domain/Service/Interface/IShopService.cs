using System.Collections.Generic;
using Game.UI.Data;
using Game.UI.Model;
using R3;

namespace Game.UI.Service
{
    public interface IShopService
    {
        ReadOnlyReactiveProperty<IReadOnlyList<ShopOfferModel>> RealMoneyOffers { get; }
        ReadOnlyReactiveProperty<IReadOnlyList<ShopOfferModel>> CurrencyOffers { get; }
        Observable<PurchaseResult> BuyRealMoneyOffer(string offerId);
        PurchaseResult BuyCurrencyOffer(string offerId);
    }
}