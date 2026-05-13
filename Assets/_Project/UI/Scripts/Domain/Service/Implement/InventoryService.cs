using System;
using System.Collections.Generic;
using System.Linq;
using Game.UI.Data;
using Game.UI.Model;
using UnityEngine;

namespace Game.UI.Service
{
    public sealed class InventoryService : IInventoryService
    {
        private readonly IPlayerProgressService _progressService;
        private readonly Dictionary<string, EquipmentItemModel> _itemsById;
        private readonly Dictionary<EquipmentSlot, EquipmentItemModel> _equipped = new();

        public event Action Changed;

        public IReadOnlyDictionary<EquipmentSlot, EquipmentItemModel> EquippedItems => new Dictionary<EquipmentSlot, EquipmentItemModel>(_equipped);
        public IReadOnlyList<EquipmentItemModel> CatalogItems => _itemsById.Values.ToArray();
        public EquipmentTab SelectedTab { get; private set; } = EquipmentTab.All;
        public IReadOnlyList<EquipmentItemModel> FilteredItems => FilterItems(SelectedTab);

        public InventoryService(IEquipmentCatalog equipmentCatalog, IPlayerProgressService progressService)
        {
            _progressService = progressService;
            _itemsById = equipmentCatalog.GetAllItems().ToDictionary(item => item.ItemId);
            ApplyProgress();
        }

        public EquipmentItemModel GetItem(string itemId)
        {
            return !string.IsNullOrWhiteSpace(itemId) && _itemsById.TryGetValue(itemId, out EquipmentItemModel item) ? item : null;
        }

        public void SelectTab(EquipmentTab tab)
        {
            SelectedTab = tab;
            Changed?.Invoke();
        }

        public bool CanEquip(string itemId)
        {
            EquipmentItemModel item = GetItem(itemId);
            return item != null && item.IsUnlocked;
        }

        public bool Equip(string itemId)
        {
            EquipmentItemModel item = GetItem(itemId);
            if (item == null)
            {
                Debug.LogError($"[InventoryService] Cannot equip missing item: {itemId}");
                return false;
            }

            if (!item.IsUnlocked)
            {
                Debug.LogWarning($"[InventoryService] Cannot equip locked item: {itemId}");
                return false;
            }

            _equipped[item.Slot] = item;
            _progressService.SetEquippedItem(item.Slot, item.ItemId);
            Changed?.Invoke();
            return true;
        }

        public bool Unequip(EquipmentSlot slot)
        {
            bool removed = _equipped.Remove(slot);
            if (removed)
            {
                _progressService.SetEquippedItem(slot, null);
                Changed?.Invoke();
            }

            return removed;
        }

        public bool Unlock(string itemId)
        {
            EquipmentItemModel item = GetItem(itemId);
            if (item == null)
            {
                return false;
            }

            item.IsUnlocked = true;
            _progressService.SetEquipmentUnlocked(item.ItemId, true);
            Changed?.Invoke();
            return true;
        }

        public bool Lock(string itemId)
        {
            EquipmentItemModel item = GetItem(itemId);
            if (item == null)
            {
                return false;
            }

            item.IsUnlocked = false;
            _progressService.SetEquipmentUnlocked(item.ItemId, false);

            if (_equipped.TryGetValue(item.Slot, out EquipmentItemModel equipped) && equipped.ItemId == item.ItemId)
            {
                _equipped.Remove(item.Slot);
                _progressService.SetEquippedItem(item.Slot, null);
            }

            Changed?.Invoke();
            return true;
        }

        public bool IsEquipped(string itemId)
        {
            EquipmentItemModel item = GetItem(itemId);
            return item != null
                   && _equipped.TryGetValue(item.Slot, out EquipmentItemModel equipped)
                   && equipped.ItemId == item.ItemId;
        }

        private IReadOnlyList<EquipmentItemModel> FilterItems(EquipmentTab tab)
        {
            if (tab == EquipmentTab.All)
            {
                return _itemsById.Values.ToArray();
            }

            EquipmentSlot slot = ToSlot(tab);
            return _itemsById.Values.Where(item => item.Slot == slot).ToArray();
        }

        private static EquipmentSlot ToSlot(EquipmentTab tab)
        {
            return tab switch
            {
                EquipmentTab.Armor => EquipmentSlot.Armor,
                EquipmentTab.Helmet => EquipmentSlot.Helmet,
                EquipmentTab.Boots => EquipmentSlot.Boots,
                EquipmentTab.Gloves => EquipmentSlot.Gloves,
                EquipmentTab.Pants => EquipmentSlot.Pants,
                EquipmentTab.Weapon => EquipmentSlot.Weapon,
                _ => EquipmentSlot.Weapon
            };
        }

        private void ApplyProgress()
        {
            foreach (EquipmentItemModel item in _itemsById.Values)
            {
                item.IsUnlocked = _progressService.IsEquipmentUnlocked(item.ItemId, item.IsUnlocked);
            }

            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                string itemId = _progressService.GetEquippedItemId(slot);
                EquipmentItemModel item = GetItem(itemId);
                if (item != null && item.IsUnlocked && item.Slot == slot)
                {
                    _equipped[slot] = item;
                }
            }
        }
    }
}
