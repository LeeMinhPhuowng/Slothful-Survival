using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Inventory
{
    /// <summary>
    /// Displays a grid visual preview outline when dragging an item.
    /// Shows valid (green) or invalid (red) slots based on overlapping grid positions.
    /// </summary>
    public class GridVisualController : MonoBehaviour
    {
        private ItemModel _draggingItem;
        private InventoryModel _sourceInventory;
        private Vector2Int _gridPositionOffset;
        private ItemSO.Dir _currentDir;

        private readonly List<GameObject> _pool = new List<GameObject>();

        private void Start()
        {
            if (DragDropSystem.Instance != null)
            {
                DragDropSystem.Instance.OnDragStart += HandleDragStart;
                DragDropSystem.Instance.OnDragEnd += HandleDragEnd;
                DragDropSystem.Instance.OnRotationChanged += HandleRotationChanged;
            }
        }

        private void OnDestroy()
        {
            if (DragDropSystem.Instance != null)
            {
                DragDropSystem.Instance.OnDragStart -= HandleDragStart;
                DragDropSystem.Instance.OnDragEnd -= HandleDragEnd;
                DragDropSystem.Instance.OnRotationChanged -= HandleRotationChanged;
            }
            ClearPool();
        }

        private void HandleDragStart(ItemModel item, InventoryModel sourceInventory, Vector2Int gridPositionOffset, ItemSO.Dir dir)
        {
            _draggingItem = item;
            _sourceInventory = sourceInventory;
            _gridPositionOffset = gridPositionOffset;
            _currentDir = dir;

            if (sourceInventory != null && sourceInventory.GetConfig() != null)
            {
                InitializePool(sourceInventory.GetConfig().GridVisualPrefab);
            }
        }

        private void HandleDragEnd()
        {
            _draggingItem = null;
            _sourceInventory = null;
            HideAllVisuals();
            ClearPool();
        }

        private void HandleRotationChanged(ItemSO.Dir newDir)
        {
            _currentDir = newDir;
        }

        private void Update()
        {
            if (_draggingItem == null) return;

            HideAllVisuals();

            // 1. Find hover inventory view under cursor
            InventoryView hoverView = null;
            Vector2Int candidateOrigin = Vector2Int.zero;
            Vector2 mousePos = Input.mousePosition;

            if (DragDropSystem.Instance != null && DragDropSystem.Instance.InventoryViews != null)
            {
                foreach (var view in DragDropSystem.Instance.InventoryViews)
                {
                    if (view == null || view.Model == null) continue;

                    var model = view.Model;
                    var container = view.ItemContainer;

                    Camera cam = null;
                    Canvas canvas = container.GetComponentInParent<Canvas>();
                    if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    {
                        cam = Camera.main;
                    }

                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        container, mousePos, cam, out Vector2 localPoint))
                    {
                        Vector2Int gridPos = model.GetGridPosition(localPoint);
                        if (model.IsValidGridPosition(gridPos))
                        {
                            hoverView = view;
                            candidateOrigin = gridPos - _gridPositionOffset;
                            break;
                        }
                    }
                }
            }

            if (hoverView == null) return;

            // 2. Get cells according to current direction shape
            ItemSO itemSO = _draggingItem.ItemSO;
            List<Vector2Int> cells = itemSO.GetGridPositionList(candidateOrigin, _currentDir);

            // 3. Render visual outline for each cell
            var gridModel = hoverView.Model.GetGrid();
            float cellSize = gridModel.GetCellSize();

            for (int i = 0; i < cells.Count; i++)
            {
                if (i >= _pool.Count) break;

                Vector2Int cell = cells[i];
                GameObject visual = _pool[i];

                visual.transform.SetParent(hoverView.ItemContainer, false);
                visual.SetActive(true);

                var rect = visual.GetComponent<RectTransform>();
                rect.anchoredPosition = gridModel.GetWorldPosition(cell.x, cell.y);
                rect.sizeDelta = new Vector2(cellSize, cellSize);

                // Check cell placement validity
                bool isValid = true;
                
                if (!hoverView.Model.IsValidGridPosition(cell))
                {
                    isValid = false;
                }
                else
                {
                    GridCell gridCell = hoverView.Model.GetGridCell(cell.x, cell.y);
                    if (itemSO.itemType == ItemType.Bag)
                    {
                        if (gridCell.HasBag()) isValid = false;
                    }
                    else
                    {
                        if (!hoverView.Model.BypassBagRequirement && !gridCell.HasBag()) isValid = false;
                        if (gridCell.HasRegular()) isValid = false;
                    }
                }

                // Apply dynamic feedback colors (Green for valid, Red for invalid)
                var img = visual.GetComponent<Image>();
                if (img != null)
                {
                    img.color = isValid ? new Color(0.2f, 1f, 0.2f, 0.5f) : new Color(1f, 0.2f, 0.2f, 0.5f);
                }
            }
        }

        private void InitializePool(Transform prefab)
        {
            ClearPool();
            if (prefab == null) return;

            for (int i = 0; i < 16; i++)
            {
                Transform cell = Instantiate(prefab, transform);
                cell.gameObject.SetActive(false);
                
                // Ensure the layout component does not block Raycasts
                var img = cell.GetComponent<Image>();
                if (img != null)
                {
                    img.raycastTarget = false;
                }
                var cg = cell.GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = cell.gameObject.AddComponent<CanvasGroup>();
                }
                cg.blocksRaycasts = false;
                cg.interactable = false;

                _pool.Add(cell.gameObject);
            }
        }

        private void HideAllVisuals()
        {
            foreach (var go in _pool)
            {
                if (go != null)
                {
                    go.SetActive(false);
                    go.transform.SetParent(transform, false);
                }
            }
        }

        private void ClearPool()
        {
            foreach (var go in _pool)
            {
                if (go != null) Destroy(go);
            }
            _pool.Clear();
        }
    }
}
