using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Inventory
{
    /// <summary>
    /// Core inventory logic. Pure C# model – no visuals, no MonoBehaviour.
    /// Communicates state changes via IEventBus.
    /// </summary>
    public class InventoryModel
    {
        private readonly GridModel<GridCell> _grid;
        private readonly InventoryConfigSO _config;
        private readonly HashSet<ItemModel> _items = new HashSet<ItemModel>();

        public bool BypassBagRequirement { get; set; } = false;

        public event Action<ItemModel> OnItemPlaced;
        public event Action<ItemModel> OnItemRemoved;

        public InventoryModel(InventoryConfigSO config)
        {
            _config = config;
            _grid = new GridModel<GridCell>(
                config.GridWidth,
                config.GridHeight,
                config.CellSize,
                Vector3.zero,
                (g, x, y) => new GridCell(g, x, y)
            );
        }

        #region Public API

        /// <summary>
        /// Checks if an item can be placed at the given origin and direction.
        /// </summary>
        public bool CanPlaceItem(ItemSO itemSO, Vector2Int origin, ItemSO.Dir dir)
        {
            List<Vector2Int> positions = itemSO.GetGridPositionList(origin, dir);

            foreach (Vector2Int pos in positions)
            {
                if (!_grid.IsValidGridPosition(pos)) return false;
                GridCell cell = _grid.GetGridObject(pos.x, pos.y);
                
                if (itemSO.itemType == ItemType.Bag)
                {
                    if (cell.HasBag()) return false; // Không được đè túi lên túi
                }
                else // Regular
                {
                    if (!BypassBagRequirement && !cell.HasBag()) return false; // Phải có túi ở dưới
                    if (cell.HasRegular()) return false; // Không được đè item lên item
                }
            }
            return true;
        }

        /// <summary>
        /// Attempts to place an item on the grid. 
        /// Publishes ItemPlacedEvent on success.
        /// </summary>
        public bool TryPlaceItem(ItemSO itemSO, Vector2Int origin, ItemSO.Dir dir)
        {
            if (!CanPlaceItem(itemSO, origin, dir)) return false;

            List<Vector2Int> positions = itemSO.GetGridPositionList(origin, dir);

            // 2. Create model & occupy cells
            var item = new ItemModel(itemSO, origin, dir);

            foreach (Vector2Int pos in positions)
            {
                _grid.GetGridObject(pos.x, pos.y).SetItem(item);
            }

            _items.Add(item);

            // 3. Notify
            OnItemPlaced?.Invoke(item);
            return true;
        }

        public bool CanRemoveItem(ItemModel item)
        {
            if (item == null || !_items.Contains(item)) return false;
            
            // Không thể nhấc túi nếu có item nằm trên
            if (item.ItemSO.itemType == ItemType.Bag)
            {
                List<Vector2Int> positions = item.GetGridPositionList();
                foreach (Vector2Int pos in positions)
                {
                    GridCell cell = _grid.GetGridObject(pos.x, pos.y);
                    if (cell != null && cell.HasRegular()) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Removes the specific item.
        /// Publishes ItemRemovedEvent on success.
        /// </summary>
        public bool RemoveItem(ItemModel item)
        {
            if (!CanRemoveItem(item)) return false;

            // Clear all cells this item occupies
            List<Vector2Int> positions = item.GetGridPositionList();
            ItemType type = item.ItemSO.itemType;
            foreach (Vector2Int pos in positions)
            {
                _grid.GetGridObject(pos.x, pos.y).ClearItem(type);
            }

            _items.Remove(item);

            // Notify
            OnItemRemoved?.Invoke(item);
            return true;
        }

        #endregion

        #region Grid Queries

        public GridModel<GridCell> GetGrid() => _grid;
        public InventoryConfigSO GetConfig() => _config;
        public IEnumerable<ItemModel> GetItems() => _items;

        public Vector2Int GetGridPosition(Vector3 worldPosition)
        {
            _grid.GetXY(worldPosition, out int x, out int y);
            return new Vector2Int(x, y);
        }

        public bool IsValidGridPosition(Vector2Int gridPosition)
        {
            return _grid.IsValidGridPosition(gridPosition);
        }

        public GridCell GetGridCell(int x, int y)
        {
            return _grid.GetGridObject(x, y);
        }

        public GridCell GetGridCell(Vector3 worldPos)
        {
            return _grid.GetGridObject(worldPos);
        }

        /// <summary>
        /// Calculates the world position for an item placed at the given origin with the given direction.
        /// </summary>
        public Vector3 GetItemWorldPosition(Vector2Int origin, ItemSO.Dir dir, ItemSO itemSO)
        {
            Vector2Int rotationOffset = itemSO.GetRotationOffset(dir);
            return _grid.GetWorldPosition(origin.x, origin.y)
                   + new Vector3(rotationOffset.x, rotationOffset.y) * _grid.GetCellSize();
        }

        #endregion

        #region Save / Load

        public string Save()
        {
            List<ItemModel.SaveData> saveList = new List<ItemModel.SaveData>();
            foreach (var item in _items)
            {
                saveList.Add(item.ToSaveData());
            }
            return JsonUtility.ToJson(new SaveDataList { items = saveList });
        }

        public void Load(string json)
        {
            if (string.IsNullOrEmpty(json)) return;

            SaveDataList data = JsonUtility.FromJson<SaveDataList>(json);
            foreach (var entry in data.items)
            {
                ItemSO so = _config.GetItemSOByName(entry.itemSOName);
                if (so != null)
                {
                    TryPlaceItem(so, entry.origin, entry.dir);
                }
            }
        }

        [Serializable]
        private class SaveDataList
        {
            public List<ItemModel.SaveData> items;
        }

        #endregion
    }
}
