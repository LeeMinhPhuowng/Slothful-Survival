using UnityEngine;

namespace Features.Inventory
{
    /// <summary>
    /// Published via IEventBus when a user starts dragging an item.
    /// </summary>
    public readonly struct ItemDragStartEvent
    {
        public readonly ItemModel Item;
        public readonly InventoryModel SourceInventory;
        public readonly Vector2 AnchoredPositionOffset;
        public readonly Vector2Int GridPositionOffset;
        public readonly ItemSO.Dir Dir;

        public ItemDragStartEvent(
            ItemModel item,
            InventoryModel sourceInventory,
            Vector2 anchoredPositionOffset,
            Vector2Int gridPositionOffset,
            ItemSO.Dir dir)
        {
            Item = item;
            SourceInventory = sourceInventory;
            AnchoredPositionOffset = anchoredPositionOffset;
            GridPositionOffset = gridPositionOffset;
            Dir = dir;
        }
    }
}
