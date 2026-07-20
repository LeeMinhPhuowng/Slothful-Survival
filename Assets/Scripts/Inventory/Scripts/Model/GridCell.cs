namespace Features.Inventory
{
    /// <summary>
    /// Represents a single cell in the inventory grid.
    /// Tracks Bag and Regular layers.
    /// </summary>
    public class GridCell
    {
        private readonly GridModel<GridCell> _grid;
        private readonly int _x;
        private readonly int _y;
        private ItemModel _bagItem;
        private ItemModel _regularItem;

        public GridCell(GridModel<GridCell> grid, int x, int y)
        {
            _grid = grid;
            _x = x;
            _y = y;
            _bagItem = null;
            _regularItem = null;
        }

        public void SetItem(ItemModel item)
        {
            if (item.ItemSO.itemType == ItemType.Bag)
                _bagItem = item;
            else
                _regularItem = item;
            _grid.TriggerGridObjectChanged(_x, _y);
        }

        public void ClearItem(ItemType type)
        {
            if (type == ItemType.Bag)
                _bagItem = null;
            else
                _regularItem = null;
            _grid.TriggerGridObjectChanged(_x, _y);
        }

        public ItemModel GetItem(ItemType type)
        {
            return type == ItemType.Bag ? _bagItem : _regularItem;
        }

        public bool HasBag() => _bagItem != null;
        public bool HasRegular() => _regularItem != null;
        
        public ItemModel GetBagItem() => _bagItem;
        public ItemModel GetRegularItem() => _regularItem;
    }
}
