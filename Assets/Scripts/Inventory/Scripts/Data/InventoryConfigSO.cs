using UnityEngine;

namespace Features.Inventory
{
    [CreateAssetMenu(menuName = "Inventory/Config")]
    public class InventoryConfigSO : ScriptableObject
    {
        [Header("Grid Settings")]
        [SerializeField] private int gridWidth = 10;
        [SerializeField] private int gridHeight = 10;
        [SerializeField] private float cellSize = 50f;

        [Header("Assets")]
        [SerializeField] private ItemSO[] allItems;
        [SerializeField] private Transform gridVisualPrefab;

        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public float CellSize => cellSize;
        public ItemSO[] AllItems => allItems;
        public Transform GridVisualPrefab => gridVisualPrefab;

        /// <summary>
        /// Tìm ItemSO theo tên (dùng cho Save/Load).
        /// </summary>
        public ItemSO GetItemSOByName(string itemName)
        {
            if (allItems == null) return null;
            foreach (var item in allItems)
            {
                if (item != null && item.name == itemName)
                    return item;
            }
            return null;
        }
    }
}
