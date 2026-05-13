using Game.UI.Data;
using UnityEngine;

namespace Game.UI.Model
{
    public sealed class ShopOfferModel
    {
        public string OfferId;
        public string DisplayName;
        public Sprite Icon;
        public CurrencyType? PriceCurrency;
        public int PriceAmount;
        public string RealMoneyProductId;
        public RewardPayload Reward;
    }
}