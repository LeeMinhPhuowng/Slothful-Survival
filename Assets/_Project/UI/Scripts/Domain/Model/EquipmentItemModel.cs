using Game.UI.Data;
using UnityEngine;

namespace Game.UI.Model
{
    public sealed class EquipmentItemModel
    {
        public string ItemId;
        public string DisplayName;
        public string Description;
        public RarityItem Rarity;
        public Sprite Icon;
        public EquipmentSlot Slot;
        public int Level;
        public int BuyPrice;
        public int SellPrice;
        public bool IsUnlocked;
        public int Armor;
        public int Damage;
        public int MaxHealth;
        public int MoveSpeed;
    } 
}
