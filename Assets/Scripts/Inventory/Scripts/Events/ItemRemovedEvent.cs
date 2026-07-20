namespace Features.Inventory
{
    /// <summary>
    /// Published via IEventBus when an item is removed from the grid.
    /// </summary>
    public readonly struct ItemRemovedEvent
    {
        public readonly ItemModel Item;
        public readonly InventoryModel Inventory;

        public ItemRemovedEvent(ItemModel item, InventoryModel inventory)
        {
            Item = item;
            Inventory = inventory;
        }
    }
}
