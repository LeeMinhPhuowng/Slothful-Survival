namespace Features.Inventory
{
    /// <summary>
    /// Published via IEventBus when an item is successfully placed on the grid.
    /// </summary>
    public readonly struct ItemPlacedEvent
    {
        public readonly ItemModel Item;
        public readonly InventoryModel Inventory;

        public ItemPlacedEvent(ItemModel item, InventoryModel inventory)
        {
            Item = item;
            Inventory = inventory;
        }
    }
}
