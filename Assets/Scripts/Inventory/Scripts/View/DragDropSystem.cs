using System.Collections.Generic;
using UnityEngine;

namespace Features.Inventory
{
    /// <summary>
    /// Orchestrates drag-drop operations across all InventoryViews.
    /// Manages dragging state via direct static singleton calls instead of EventBus.
    /// </summary>
    public class DragDropSystem : MonoBehaviour
    {
        public static DragDropSystem Instance { get; private set; }

        [Header("All inventory views in the scene")]
        [SerializeField] private List<InventoryView> _inventoryViews;

        // Drag state
        private ItemModel _draggingItem;
        private InventoryModel _sourceInventory;
        private Vector2 _anchoredPositionOffset;
        private Vector2Int _gridPositionOffset;
        private ItemSO.Dir _currentDir;
        private ItemDragHandler _activeDragHandler;

        // Events for GridVisualController
        public event System.Action<ItemModel, InventoryModel, Vector2Int, ItemSO.Dir> OnDragStart;
        public event System.Action OnDragEnd;
        public event System.Action<ItemSO.Dir> OnRotationChanged;

        public List<InventoryView> InventoryViews => _inventoryViews;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Ensure GridVisualController is attached
            if (GetComponent<GridVisualController>() == null)
            {
                gameObject.AddComponent<GridVisualController>();
            }
        }

        private void Update()
        {
            if (_draggingItem != null && Input.GetKeyDown(KeyCode.R))
            {
                _currentDir = ItemSO.GetNextDir(_currentDir);
                if (_activeDragHandler != null)
                {
                    _activeDragHandler.RotateTo(_currentDir);
                }
                OnRotationChanged?.Invoke(_currentDir);
            }
        }

        /// <summary>
        /// Initiates dragging of an item. Called by ItemDragHandler.
        /// </summary>
        public void HandleDragStart(ItemModel item, InventoryModel sourceInventory, Vector2 anchoredPositionOffset, Vector2Int gridPositionOffset, ItemSO.Dir dir, ItemDragHandler dragHandler)
        {
            _draggingItem = item;
            _sourceInventory = sourceInventory;
            _anchoredPositionOffset = anchoredPositionOffset;
            _gridPositionOffset = gridPositionOffset;
            _currentDir = dir;
            _activeDragHandler = dragHandler;
            OnDragStart?.Invoke(item, sourceInventory, gridPositionOffset, dir);
        }

        /// <summary>
        /// Concludes dragging of an item. Called by ItemDragHandler.
        /// </summary>
        public void HandleDragEnd(ItemModel item, InventoryModel sourceInventory, Vector2 screenPosition, Camera eventCamera, RectTransform itemVisualRect)
        {
            if (_draggingItem == null) return;

            var originalOrigin = item.Origin;
            var originalDir = item.Dir;

            _draggingItem = null;
            _sourceInventory = null;
            _activeDragHandler = null;

            // Remove from source
            sourceInventory.RemoveItem(item);

            // Find target inventory under cursor
            InventoryModel targetInv = null;
            Vector2Int targetOrigin = Vector2Int.zero;

            foreach (var view in _inventoryViews)
            {
                if (view == null || view.Model == null) continue;

                var model = view.Model;
                var container = view.ItemContainer;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    container, screenPosition, eventCamera, out Vector2 localPoint);

                Vector2Int gridPos = model.GetGridPosition(localPoint);

                if (model.IsValidGridPosition(gridPos))
                {
                    targetInv = model;

                    // Align target origin 100% perfectly with the preview cells shown by the GridVisualController
                    targetOrigin = gridPos - _gridPositionOffset;
                    break;
                }
            }

            // Try place on target
            bool placed = false;
            if (targetInv != null)
            {
                placed = targetInv.TryPlaceItem(item.ItemSO, targetOrigin, _currentDir);
            }

            // Fallback → return to original position
            if (!placed)
            {
                sourceInventory.TryPlaceItem(item.ItemSO, originalOrigin, originalDir);
            }

            OnDragEnd?.Invoke();
        }
    }
}
