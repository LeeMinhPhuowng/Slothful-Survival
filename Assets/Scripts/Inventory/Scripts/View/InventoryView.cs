using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Inventory
{
    /// <summary>
    /// Visual representation of an inventory grid.
    /// Each InventoryView owns its own InventoryModel (1:1).
    /// Subscribes to event callbacks for item placement/removal to manage ItemView instances.
    /// </summary>
    public class InventoryView : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private InventoryConfigSO _config;

        [Header("UI References")]
        [SerializeField] private RectTransform _itemContainer;
        [SerializeField] private Transform _backgroundContainer;
        [SerializeField] private Transform _backgroundTemplate;

        private InventoryModel _inventory;
        private readonly Dictionary<ItemModel, ItemView> _itemViews = new Dictionary<ItemModel, ItemView>();
        private ItemView _selectedItemView;

        /// <summary> The model this view manages. </summary>
        public InventoryModel Model
        {
            get
            {
                if (_inventory == null && _config != null)
                {
                    _inventory = new InventoryModel(_config);
                }
                return _inventory;
            }
        }
        public RectTransform ItemContainer => _itemContainer;

        private void Start()
        {
            if (_config == null)
            {
                Debug.LogError("[InventoryView] InventoryConfigSO chưa được gán!", this);
                return;
            }

            // 1. Tạo model – mỗi InventoryView sở hữu một InventoryModel riêng nếu chưa được tạo
            if (_inventory == null)
            {
                _inventory = new InventoryModel(_config);
            }

            // 2. Subscribe events
            _inventory.OnItemPlaced += OnItemPlaced;
            _inventory.OnItemRemoved += OnItemRemoved;
            ItemDragHandler.OnItemSelected += OnItemSelected;

            // Cưỡng ép ItemContainer (_itemContainer) bao phủ hoàn toàn View với offset bằng 0
            if (_itemContainer != null)
            {
                _itemContainer.anchorMin = Vector2.zero;
                _itemContainer.anchorMax = Vector2.one;
                _itemContainer.pivot = new Vector2(0f, 0f); // Ép pivot về (0,0) (Bottom-Left) để trùng khớp toạ độ lưới chuẩn
                _itemContainer.offsetMin = Vector2.zero;
                _itemContainer.offsetMax = Vector2.zero;
                _itemContainer.anchoredPosition = Vector2.zero;
            }

            // 3. Build background grid
            BuildBackground();

            // 4. Vẽ toàn bộ vật phẩm đã tồn tại sẵn trong dữ liệu (Model) từ trước
            foreach (var item in _inventory.GetItems())
            {
                OnItemPlaced(item);
            }
        }

        #region Background

        private void BuildBackground()
        {
            if (_backgroundTemplate == null || _backgroundContainer == null)
            {
                Debug.LogWarning("[InventoryView] Background template/container chưa được gán, bỏ qua vẽ nền.", this);
                return;
            }

            var grid = _inventory.GetGrid();
            _backgroundTemplate.gameObject.SetActive(false);

            // 1. Dọn dẹp toàn bộ ô cũ trong Container trước khi vẽ mới để tránh tràn ô và bị nhân đôi ô
            var childrenToDestroy = new List<GameObject>();
            foreach (Transform child in _backgroundContainer)
            {
                if (child == _backgroundTemplate) continue;
                childrenToDestroy.Add(child.gameObject);
            }
            foreach (var childGo in childrenToDestroy)
            {
                childGo.SetActive(false); // Disable ngay lập tức để GridLayoutGroup không tính toán nữa
                Destroy(childGo);
            }

            int width = grid.GetWidth();
            int height = grid.GetHeight();

            // 2. Tạo các ô mới theo đúng kích thước Grid
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Transform cell = Instantiate(_backgroundTemplate, _backgroundContainer);
                    cell.gameObject.SetActive(true);
                }
            }

            // 3. Cấu hình GridLayoutGroup cố định số cột và kích thước ô
            var layoutGroup = _backgroundContainer.GetComponent<GridLayoutGroup>();
            if (layoutGroup != null)
            {
                layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layoutGroup.constraintCount = width;
                layoutGroup.cellSize = new Vector2(grid.GetCellSize(), grid.GetCellSize());
            }

            // 4. Cấu hình kích thước tổng thể khớp với Grid thực tế
            Vector2 gridDimensions = new Vector2(width, height) * grid.GetCellSize();
            
            var bgRect = _backgroundContainer.GetComponent<RectTransform>();
            var myRect = GetComponent<RectTransform>();

            // Đồng bộ kích thước chính xác cho View (chứa các Item)
            if (myRect != null)
            {
                // ÉP Neo (Anchors) và Tâm (Pivot) về chính giữa để sizeDelta mang giá trị kích thước TUYỆT ĐỐI (250x250)
                // Thay vì bị kéo giãn (stretch) theo khung cha dẫn đến sai kích thước.
                myRect.anchorMin = new Vector2(0.5f, 0.5f);
                myRect.anchorMax = new Vector2(0.5f, 0.5f);
                myRect.pivot = new Vector2(0.5f, 0.5f);
                myRect.sizeDelta = gridDimensions;
                myRect.anchoredPosition = Vector2.zero; // Căn giữa hoàn hảo trong hòm đồ cha (Storage/Active)
            }

            // Đồng bộ kích thước, neo (anchors), tâm xoay (pivot) và vị trí của Background khớp 100% với View
            if (bgRect != null && myRect != null)
            {
                bgRect.anchorMin = myRect.anchorMin;
                bgRect.anchorMax = myRect.anchorMax;
                bgRect.pivot = myRect.pivot;
                bgRect.sizeDelta = gridDimensions;
                bgRect.anchoredPosition = myRect.anchoredPosition; // Cũng căn giữa hoàn hảo (Vector2.zero)
            }
        }

        #endregion

        #region Event Handlers

        private void OnItemPlaced(ItemModel item)
        {
            var itemSO = item.ItemSO;

            if (itemSO.visualPrefab == null)
            {
                Debug.LogWarning($"[InventoryView] ItemSO '{itemSO.nameString}' has no visualPrefab assigned.");
                return;
            }

            // Instantiate visual
            Transform itemTransform = Instantiate(itemSO.visualPrefab, _itemContainer);
            var rectTransform = itemTransform.GetComponent<RectTransform>();

            // Chuẩn hoá anchor và pivot về (0,0) để anchoredPosition = local position chính xác,
            // khớp hoàn toàn với hệ toạ độ lưới (origin = góc dưới-trái). Nếu không làm điều này,
            // prefab có pivot=(0.5,0.5) sẽ khiến item bị trôi xa khi bắt đầu kéo.
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.pivot = Vector2.zero;

            // Tự động bo kích thước của Item UI khớp 100% với kích thước ô lưới thực tế
            rectTransform.sizeDelta = new Vector2(itemSO.width, itemSO.height) * _inventory.GetGrid().GetCellSize();

            // Position & rotate
            Vector3 worldPos = _inventory.GetItemWorldPosition(item.Origin, item.Dir, itemSO);
            rectTransform.anchoredPosition = worldPos;
            itemTransform.localRotation = Quaternion.Euler(0, 0, -itemSO.GetRotationAngle(item.Dir));

            // Initialize ItemView
            var itemView = itemTransform.GetComponent<ItemView>();
            if (itemView == null)
            {
                itemView = itemTransform.gameObject.AddComponent<ItemView>();
            }
            itemView.Initialize(item, _inventory, _itemContainer);

            // Xử lý Layer đồ hoạ: Túi hiển thị phía dưới, Item hiển thị phía trên cùng
            if (itemSO.itemType == ItemType.Bag)
            {
                itemTransform.SetAsFirstSibling();
            }
            else
            {
                itemTransform.SetAsLastSibling();
            }

            _itemViews[item] = itemView;
        }

        private void OnItemRemoved(ItemModel item)
        {
            if (_itemViews.TryGetValue(item, out ItemView view))
            {
                _itemViews.Remove(item);
                if (_selectedItemView == view)
                {
                    _selectedItemView = null;
                }
                if (view != null && view.gameObject != null)
                {
                    Destroy(view.gameObject);
                }
            }
        }

        private void OnItemSelected(ItemModel item, InventoryModel inventory)
        {
            if (inventory != Model) return;

            // Deselect old
            if (_selectedItemView != null)
            {
                _selectedItemView.Deselect();
                _selectedItemView = null;
            }

            // Select new
            if (item != null && _itemViews.TryGetValue(item, out ItemView newView))
            {
                newView.Select();
                _selectedItemView = newView;
            }
        }

        #endregion

        private void OnDestroy()
        {
            if (_inventory != null)
            {
                _inventory.OnItemPlaced -= OnItemPlaced;
                _inventory.OnItemRemoved -= OnItemRemoved;
            }
            ItemDragHandler.OnItemSelected -= OnItemSelected;
        }
    }
}
