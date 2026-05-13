using System;

namespace Game.UI.Data
{
    public readonly struct RewardPayload
    {
        public static readonly RewardPayload Empty = new(
            Array.Empty<CurrencyRewardPayload>(),
            Array.Empty<string>(),
            Array.Empty<string>());

        public readonly CurrencyRewardPayload[] Currencies;
        public readonly string[] EquipmentItemIds;
        public readonly string[] CharacterIds;

        public RewardPayload(CurrencyRewardPayload[] currencies, string[] equipmentItemIds, string[] characterIds)
        {
            Currencies = currencies ?? Array.Empty<CurrencyRewardPayload>();
            EquipmentItemIds = equipmentItemIds ?? Array.Empty<string>();
            CharacterIds = characterIds ?? Array.Empty<string>();
        }
    }
}