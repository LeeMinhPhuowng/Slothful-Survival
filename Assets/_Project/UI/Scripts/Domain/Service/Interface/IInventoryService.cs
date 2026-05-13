using System.Collections.Generic;
using System;
using Game.UI.Data;
using Game.UI.Model;

namespace Game.UI.Service
{
    public interface IInventoryService
    {
        event Action Changed;
        IReadOnlyDictionary<EquipmentSlot, EquipmentItemModel> EquippedItems { get; }
        IReadOnlyList<EquipmentItemModel> CatalogItems { get; }
        EquipmentTab SelectedTab { get; }
        IReadOnlyList<EquipmentItemModel> FilteredItems { get; }
        EquipmentItemModel GetItem(string itemId);
        void SelectTab(EquipmentTab tab);
        bool CanEquip(string itemId);
        bool Equip(string itemId);
        bool Unequip(EquipmentSlot slot);
        bool Unlock(string itemId);
        bool Lock(string itemId);
        bool IsEquipped(string itemId);
    }
}
