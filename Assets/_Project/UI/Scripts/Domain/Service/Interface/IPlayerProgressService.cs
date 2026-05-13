using R3;
using System;
using Game.UI.Data;
using Game.UI.Save;

namespace Game.UI.Service
{
    public interface IPlayerProgressService
    {
        event Action Changed;
        ReadOnlyReactiveProperty<float> ExpPercent { get; }
        ReadOnlyReactiveProperty<int> CurrentLevel { get; }
        PlayerProgressData Data { get; }
        int GetGold();
        int GetDiamond();
        void SetCurrency(CurrencyType currencyType, int amount);
        bool IsEquipmentUnlocked(string itemId, bool defaultValue = false);
        void SetEquipmentUnlocked(string itemId, bool isUnlocked);
        string GetEquippedItemId(EquipmentSlot slot);
        void SetEquippedItem(EquipmentSlot slot, string itemId);
        bool IsCharacterUnlocked(string characterId, bool defaultValue = false);
        void SetCharacterUnlocked(string characterId, bool isUnlocked);
        string GetSelectedCharacterId();
        void SetSelectedCharacter(string characterId);
        bool IsMapUnlocked(string mapId, bool defaultValue = false);
        void SetMapUnlocked(string mapId, bool isUnlocked);
        string GetSelectedMapId();
        void SetSelectedMap(string mapId);
        void Save();
    }
}
