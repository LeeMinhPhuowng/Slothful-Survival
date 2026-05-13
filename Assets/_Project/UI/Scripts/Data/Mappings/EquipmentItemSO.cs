using Game.UI.Data;
using Game.UI.Model;
using UnityEngine;

namespace Game.UI.Core
{
    [CreateAssetMenu(fileName = "Equipment Item", menuName = "Equipment/New Equipment Item")]
    public sealed class EquipmentItemSO : ScriptableObject
    {
        [Header("Identity")]
        public string ItemId;
        public string DisplayName;
        [TextArea] public string Description;
        public RarityItem Rarity;
        public Sprite Icon;
        public EquipmentSlot Slot;
        public int Level;
        public bool IsUnlocked;

        [Header("Economy")]
        [Min(0)] public int BuyPrice;
        [Min(0)] public int SellPrice;

        [Header("Stats")]
        public int Armor;
        public int Damage;
        public int MaxHealth;
        public int MoveSpeed;

        public string ResolveId()
        {
            return string.IsNullOrWhiteSpace(ItemId) ? name : ItemId;
        }

        public EquipmentItemModel ToModel()
        {
            string id = ResolveId();
            EquipmentSlot resolvedSlot = ResolveSlot(id);
            return new EquipmentItemModel
            {
                ItemId = id,
                DisplayName = string.IsNullOrWhiteSpace(DisplayName) ? id : DisplayName,
                Description = Description,
                Rarity = Rarity,
                Icon = Icon,
                Slot = resolvedSlot,
                Level = Level,
                BuyPrice = BuyPrice,
                SellPrice = SellPrice,
                IsUnlocked = IsUnlocked,
                Armor = Armor,
                Damage = Damage,
                MaxHealth = MaxHealth,
                MoveSpeed = MoveSpeed
            };
        }

        private EquipmentSlot ResolveSlot(string resolvedId)
        {
            if (TryResolveSlot(resolvedId, out EquipmentSlot inferredSlot))
            {
                return inferredSlot;
            }

            if (TryResolveSlot(name, out inferredSlot))
            {
                return inferredSlot;
            }

            return Slot;
        }

        private static bool TryResolveSlot(string source, out EquipmentSlot resolved)
        {
            string normalized = source?.Trim().ToLowerInvariant() ?? string.Empty;

            if (normalized.StartsWith("helmet"))
            {
                resolved = EquipmentSlot.Helmet;
                return true;
            }

            if (normalized.StartsWith("armor"))
            {
                resolved = EquipmentSlot.Armor;
                return true;
            }

            if (normalized.StartsWith("gloves"))
            {
                resolved = EquipmentSlot.Gloves;
                return true;
            }

            if (normalized.StartsWith("pants"))
            {
                resolved = EquipmentSlot.Pants;
                return true;
            }

            if (normalized.StartsWith("boots"))
            {
                resolved = EquipmentSlot.Boots;
                return true;
            }

            if (normalized.StartsWith("weapon")
                || normalized.StartsWith("bow")
                || normalized.StartsWith("dagger")
                || normalized.StartsWith("sword"))
            {
                resolved = EquipmentSlot.Weapon;
                return true;
            }

            resolved = default;
            return false;
        }
    }
}
