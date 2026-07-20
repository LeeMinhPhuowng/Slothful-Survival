using UnityEngine;

namespace Features.Inventory
{
    /// <summary>
    /// Visual representation of a single item on the inventory grid.
    /// Holds reference to its ItemModel for drag operations.
    /// </summary>
    public class ItemView : MonoBehaviour
    {
        public ItemModel ItemModel { get; private set; }
        public InventoryModel Inventory { get; private set; }
        public RectTransform ItemContainer { get; private set; }

        private GameObject _selectionFrame;
        private bool _isSelected;

        public bool IsSelected => _isSelected;

        [Header("Selection Visual Settings")]
        [SerializeField] private Color _cornerColor = new Color(1f, 0.85f, 0.25f, 1f); // Gold
        [SerializeField] private float _cornerLength = 12f;
        [SerializeField] private float _cornerThickness = 3f;
        [SerializeField] private float _cornerPadding = 2f;

        public void Initialize(ItemModel itemModel, InventoryModel inventory, RectTransform itemContainer)
        {
            ItemModel = itemModel;
            Inventory = inventory;
            ItemContainer = itemContainer;

            // Ensure drag handler is attached
            var dragHandler = GetComponent<ItemDragHandler>();
            if (dragHandler == null)
            {
                dragHandler = gameObject.AddComponent<ItemDragHandler>();
            }
            dragHandler.Setup(this);

            CreateSelectionFrame();
        }

        /// <summary>
        /// Updates the visual position and rotation to match model state.
        /// </summary>
        public void SyncVisualToModel()
        {
            if (ItemModel == null) return;

            Vector3 worldPos = Inventory.GetItemWorldPosition(
                ItemModel.Origin, ItemModel.Dir, ItemModel.ItemSO);

            var rect = GetComponent<RectTransform>();
            rect.anchoredPosition = worldPos;
            transform.localRotation = Quaternion.Euler(0, 0, -ItemModel.ItemSO.GetRotationAngle(ItemModel.Dir));
        }

        public void Select()
        {
            if (_isSelected) return;
            _isSelected = true;
            if (_selectionFrame != null)
            {
                _selectionFrame.SetActive(true);
            }
        }

        public void Deselect()
        {
            if (!_isSelected) return;
            _isSelected = false;
            if (_selectionFrame != null)
            {
                _selectionFrame.SetActive(false);
            }
        }

        private void CreateSelectionFrame()
        {
            if (_selectionFrame != null) return;

            // 1. Create selection frame root stretching to fill parent
            _selectionFrame = new GameObject("SelectionFrame", typeof(RectTransform));
            var rootRect = _selectionFrame.GetComponent<RectTransform>();
            rootRect.SetParent(transform, false);
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var cg = _selectionFrame.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            cg.interactable = false;

            // 2. Create 4 L-shaped corner indicators
            CreateCorner("Corner_TL", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(_cornerPadding, -_cornerPadding));
            CreateCorner("Corner_TR", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-_cornerPadding, -_cornerPadding));
            CreateCorner("Corner_BL", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(_cornerPadding, _cornerPadding));
            CreateCorner("Corner_BR", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-_cornerPadding, _cornerPadding));

            _selectionFrame.SetActive(false);
        }

        private void CreateCorner(string name, Vector2 anchor, Vector2 pivot, Vector2 anchoredPos)
        {
            GameObject cornerGo = new GameObject(name, typeof(RectTransform));
            var rect = cornerGo.GetComponent<RectTransform>();
            rect.SetParent(_selectionFrame.transform, false);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.sizeDelta = new Vector2(_cornerLength, _cornerLength);
            rect.anchoredPosition = anchoredPos;

            // Horizontal Bar
            GameObject barH = new GameObject("Bar_H", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            var hRect = barH.GetComponent<RectTransform>();
            hRect.SetParent(rect, false);
            hRect.anchorMin = pivot;
            hRect.anchorMax = pivot;
            hRect.pivot = pivot;
            hRect.sizeDelta = new Vector2(_cornerLength, _cornerThickness);
            hRect.anchoredPosition = Vector2.zero;
            var hImg = barH.GetComponent<UnityEngine.UI.Image>();
            hImg.color = _cornerColor;
            hImg.raycastTarget = false;

            // Vertical Bar
            GameObject barV = new GameObject("Bar_V", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            var vRect = barV.GetComponent<RectTransform>();
            vRect.SetParent(rect, false);
            vRect.anchorMin = pivot;
            vRect.anchorMax = pivot;
            vRect.pivot = pivot;
            vRect.sizeDelta = new Vector2(_cornerThickness, _cornerLength);
            vRect.anchoredPosition = Vector2.zero;
            var vImg = barV.GetComponent<UnityEngine.UI.Image>();
            vImg.color = _cornerColor;
            vImg.raycastTarget = false;
        }
    }
}
