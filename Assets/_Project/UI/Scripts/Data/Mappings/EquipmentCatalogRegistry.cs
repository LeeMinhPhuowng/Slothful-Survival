using System.Collections.Generic;
using System.Linq;
using Game.UI.Model;
using Game.UI.Service;
using UnityEngine;

namespace Game.UI.Core
{
    public sealed class EquipmentCatalogRegistry : MonoBehaviour, IEquipmentCatalog
    {
        [SerializeField] private List<EquipmentItemSO> items = new();

        private readonly Dictionary<string, EquipmentItemModel> _itemsById = new();
        private bool _isBuilt;

        private void Awake()
        {
            Build();
        }

        public EquipmentItemModel GetItem(string itemId)
        {
            EnsureBuilt();
            return !string.IsNullOrWhiteSpace(itemId) && _itemsById.TryGetValue(itemId, out EquipmentItemModel item)
                ? Clone(item)
                : null;
        }

        public IReadOnlyList<EquipmentItemModel> GetAllItems()
        {
            EnsureBuilt();
            return _itemsById.Values.Select(Clone).ToArray();
        }

        private void EnsureBuilt()
        {
            if (_isBuilt)
            {
                return;
            }

            Build();
        }

        private void Build()
        {
            _itemsById.Clear();

            foreach (EquipmentItemSO item in items)
            {
                if (item == null)
                {
                    continue;
                }

                EquipmentItemModel model = item.ToModel();
                if (string.IsNullOrWhiteSpace(model.ItemId) || _itemsById.ContainsKey(model.ItemId))
                {
                    Debug.LogError($"[EquipmentCatalogRegistry] Invalid or duplicate equipment id: {model.ItemId}");
                    continue;
                }

                _itemsById.Add(model.ItemId, model);
            }

            _isBuilt = true;
        }

        private static EquipmentItemModel Clone(EquipmentItemModel source)
        {
            if (source == null)
            {
                return null;
            }

            return new EquipmentItemModel
            {
                ItemId = source.ItemId,
                DisplayName = source.DisplayName,
                Description = source.Description,
                Rarity = source.Rarity,
                Icon = source.Icon,
                Slot = source.Slot,
                Level = source.Level,
                BuyPrice = source.BuyPrice,
                SellPrice = source.SellPrice,
                IsUnlocked = source.IsUnlocked,
                Armor = source.Armor,
                Damage = source.Damage,
                MaxHealth = source.MaxHealth,
                MoveSpeed = source.MoveSpeed
            };
        }
    }
}
