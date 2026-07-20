using UnityEngine;

namespace Features.Inventory
{
    /// <summary>
    /// Published via IEventBus when a user finishes dragging an item (releases mouse/touch).
    /// </summary>
    public readonly struct ItemDragEndEvent
    {
        public readonly ItemModel Item;
        public readonly InventoryModel SourceInventory;
        public readonly Vector3 ScreenPosition;

        public ItemDragEndEvent(ItemModel item, InventoryModel sourceInventory, Vector3 screenPosition)
        {
            Item = item;
            SourceInventory = sourceInventory;
            ScreenPosition = screenPosition;
        }
    }
}
